using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using BLCTWeb.Models;
using BLCTWeb.Stores;
using Bulk_Log_Comparison_Tool.LibraryClasses;
using Bulk_Log_Comparison_Tool.DataClasses;

namespace BLCTWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RoleCheckController : ControllerBase
    {
        private static readonly RankStore _store = new RankStore();

        // GET: api/rolecheck/analyze?url={logUrl}&account={accountName}
        [HttpGet("analyze")]
        public async Task<ActionResult<RankDto?>> Analyze([FromQuery][Required] string url, [FromQuery][Required] string account, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(account))
                return BadRequest(new { message = "Both 'url' and 'account' are required." });

            string? tempPath = null;
            try
            {
                tempPath = await DownloadToTempAsync(url, ct);

                // Choose parser by extension or content
                IParsedEvtcLog log = ParseLog(tempPath);

                // Validate account in log
                if (!log.HasPlayer(account))
                    return BadRequest(new { message = $"Account '{account}' not found in log." });

                // Evaluate all ranks and pick the highest eligible
                var allRanks = _store.GetAll().Select(kv => kv.Value).ToList();
                var eligible = allRanks.Where(r => IsEligible(log, r, account)).ToList();
                if (eligible.Count == 0)
                    return NotFound(new { message = "No eligible rank found for this log and account." });

                // Define "highest" as the one with the largest BossHealthPercent (tie-break by name)
                var best = eligible
                    .OrderByDescending(r => r.BossHealthPercent)
                    .ThenBy(r => r.Name, StringComparer.OrdinalIgnoreCase)
                    .First();

                return Ok(RankDto.From(best));
            }
            catch (OperationCanceledException)
            {
                return StatusCode(499, new { message = "Request canceled." });
            }
            catch (Exception ex)
            {
                return Problem($"Analysis failed: {ex.Message}", statusCode: 500);
            }
            finally
            {
                if (!string.IsNullOrEmpty(tempPath))
                {
                    try { System.IO.File.Delete(tempPath); } catch { /* ignore */ }
                }
            }
        }

        // GET: api/rolecheck/ranks
        [HttpGet("ranks")]
        public ActionResult<IEnumerable<RankDto>> GetAll()
        {
            var result = _store.GetAll()
                .Select(kv => kv.Value)
                .OrderBy(r => r.Name)
                .Select(RankDto.From)
                .ToArray();
            return Ok(result);
        }

        // GET: api/rolecheck/ranks/{id}   (id is the Discord role id, e.g. "1221047715551510578")
        [HttpGet("ranks/{id}")]
        public ActionResult<RankDto> GetById([FromRoute] string id)
        {
            if (!ulong.TryParse(id, out var key))
                return BadRequest(new { message = "Invalid id format. Expecting numeric Discord role id." });

            var rank = _store.Get(key);
            if (rank == null)
                return NotFound();

            return Ok(RankDto.From(rank));
        }

        // POST: api/rolecheck/ranks
        [HttpPost("ranks")]
        public ActionResult<RankDto> Create([FromBody] CreateRankRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (_store.Contains(request.DiscordRoleId))
                return Conflict(new { message = "A rank for the provided DiscordRoleId already exists." });

            var rank = new Rank
            {
                DiscordRoleId = request.DiscordRoleId,
                Name = request.Name.Trim(),
                Phase = request.Phase.Trim(),
                BossHealthPercent = request.BossHealthPercent,
                InstanceType = request.InstanceType
            };

            _store.Set(request.DiscordRoleId, rank);

            var dto = RankDto.From(rank);
            return CreatedAtAction(nameof(GetById), new { id = request.DiscordRoleId.ToString() }, dto);
        }

        // PUT: api/rolecheck/ranks/{id}
        [HttpPut("ranks/{id}")]
        public IActionResult Update([FromRoute] string id, [FromBody] UpdateRankRequest request)
        {
            if (!ulong.TryParse(id, out var key))
                return BadRequest(new { message = "Invalid id format. Expecting numeric Discord role id." });

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var existing = _store.Get(key);
            if (existing == null)
                return NotFound();

            existing.Name = request.Name.Trim();
            existing.Phase = request.Phase.Trim();
            existing.BossHealthPercent = request.BossHealthPercent;
            existing.InstanceType = request.InstanceType;

            _store.Set(key, existing); // persist

            return NoContent();
        }

        // DELETE: api/rolecheck/ranks/{id}
        [HttpDelete("ranks/{id}")]
        public IActionResult Delete([FromRoute] string id)
        {
            if (!ulong.TryParse(id, out var key))
                return BadRequest(new { message = "Invalid id format. Expecting numeric Discord role id." });

            if (!_store.Remove(key))
                return NotFound();

            return NoContent();
        }

        // Eligibility logic
        private static bool IsEligible(IParsedEvtcLog log, Rank rank, string account)
        {
            if (!log.HasPlayer(account))
                return false;

            // If a phase is configured, it must exist (support custom phase format "Name|start|duration")
            var hasPhase = log.GetPhases(Array.Empty<string>())
                .Any(p => string.Equals(p.Split('|')[0].Split(':')[0], rank.Phase, StringComparison.Ordinal));
            if (!hasPhase)
                return false;

            // Check whether boss reached the required health percentage inside the phase
            if (rank.BossHealthPercent > 0)
            {
                var reachedAt = log.GetTimeForBossHealth(rank.Phase, rank.BossHealthPercent);
                if (reachedAt < 0)
                {
                    return false;
                }
            }

            return true;
        }

        // Parse helpers
        private static IParsedEvtcLog ParseLog(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();

            // JSON export using JsonParser
            if (ext == ".json")
            {
                var jsonParser = new JsonParser();
                return jsonParser.ParseLog(path);
            }

            // EVTC/ZEvtc using LibraryParser
            // LibraryParser handles .zevtc and raw .evtc; zipped variants are typically .zevtc
            var libraryParser = new LibraryParser(false);
            return libraryParser.ParseLog(path);
        }

        // Download helpers (preserve extension if possible)
        private static async Task<string> DownloadToTempAsync(string url, CancellationToken ct)
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            var fileName = GetFileNameFromResponse(url, response);
            var ext = Path.GetExtension(fileName);
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ext);

            await using var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
            await using var stream = await response.Content.ReadAsStreamAsync(ct);
            await stream.CopyToAsync(fs, 4096, ct);

            return tempPath;
        }

        private static string GetFileNameFromResponse(string url, HttpResponseMessage response)
        {
            var cd = response.Content.Headers.ContentDisposition?.FileName?.Trim('"');
            if (!string.IsNullOrWhiteSpace(cd))
                return cd!;
            // fallback: try URL
            try
            {
                var uri = new Uri(url);
                var last = Path.GetFileName(uri.LocalPath);
                if (!string.IsNullOrWhiteSpace(last))
                    return last;
            }
            catch { /* ignore */ }
            // default: JSON to be safe
            return "log.zevtc";
        }

        // DTOs and request models

        public sealed record RankDto(
            ulong DiscordRoleId,
            string Name,
            string Phase,
            double BossHealthPercent,
            InstanceType InstanceType)
        {
            public static RankDto From(Rank r) =>
                new(r.DiscordRoleId, r.Name, r.Phase, r.BossHealthPercent, r.InstanceType);
        }

        public sealed class CreateRankRequest
        {
            [Required, MinLength(2)]
            public string Name { get; set; } = string.Empty;

            [Range(1, ulong.MaxValue)]
            public ulong DiscordRoleId { get; set; }

            [Required, MinLength(1)]
            public string Phase { get; set; } = string.Empty;

            [Range(0, 100)]
            public double BossHealthPercent { get; set; }

            [Required]
            public InstanceType InstanceType { get; set; } = InstanceType.Unknown;
        }

        public sealed class UpdateRankRequest
        {
            [Required, MinLength(2)]
            public string Name { get; set; } = string.Empty;

            [Required, MinLength(1)]
            public string Phase { get; set; } = string.Empty;

            [Range(0, 100)]
            public double BossHealthPercent { get; set; }

            [Required]
            public InstanceType InstanceType { get; set; } = InstanceType.Unknown;
        }
    }
}
