using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using BLCTWeb.Models;
using BLCTWeb.Stores;
using Bulk_Log_Comparison_Tool.LibraryClasses;
using Bulk_Log_Comparison_Tool.DataClasses;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace BLCTWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RoleCheckController : ControllerBase
    {
        private static readonly RankStore _store = new RankStore();
        private readonly ILogger<RoleCheckController> _logger;

        // Clients send SHA-256(password) in X-Admin-Auth. Change this plaintext to your secret.
        private const string AdminPasswordPlaintext = "U4)@9&g*8n%&6g&";

        public RoleCheckController(ILogger<RoleCheckController> logger)
        {
            _logger = logger;
        }

        // GET: api/rolecheck/auth/verify
        [HttpGet("auth/verify")]
        public IActionResult VerifyAdmin()
        {
            var hasHeader = Request.Headers.ContainsKey("X-Admin-Auth");
            var ok = IsAuthorizedFromHeader(Request.Headers);

            _logger.LogInformation("VerifyAdmin request from {RemoteIP}. HeaderPresent={HeaderPresent}, Result={Result}",
                HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown",
                hasHeader,
                ok);

            if (!ok)
            {
                // Log optionally the provided header length (don't log secret)
                if (hasHeader)
                {
                    var provided = Request.Headers["X-Admin-Auth"].ToString();
                    _logger.LogDebug("Provided X-Admin-Auth length: {Len}", provided?.Length ?? 0);
                }
                return Unauthorized(new { message = "Invalid admin hash." });
            }
            return NoContent();
        }

        // GET: api/rolecheck/analyze?url={logUrl}&account={accountName}
        [HttpGet("analyze")]
        public async Task<ActionResult<RankDto?>> Analyze([FromQuery][Required] string url, [FromQuery][Required] string account, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(account))
                return BadRequest(new { message = "Both 'url' and 'account' are required." });

            string? tempPath = null;
            try
            {
                // Normalize incoming URL (e.g., dps.report permalink -> getJson endpoint)
                var normalizedUrl = NormalizeLogUrl(url);

                tempPath = await DownloadToTempAsync(normalizedUrl, ct);

                IParsedEvtcLog log = ParseLog(tempPath);

                if (!log.HasPlayer(account))
                    return BadRequest(new { message = $"Account '{account}' not found in log." });

                // Detect instance from the log and use it to pre-filter ranks
                var detectedInstance = DetectInstanceType(log);

                var allRanks = _store.GetAll().Select(kv => kv.Value).ToList();
                var scoped = allRanks.Where(r =>
                    r.InstanceType == InstanceType.Unknown ||
                    r.InstanceType == InstanceType.Other ||
                    r.InstanceType == detectedInstance).ToList();

                var eligible = scoped.Where(r => IsEligible(log, r, account)).ToList();
                if (eligible.Count == 0)
                    return NotFound(new { message = "No eligible rank found for this log and account." });

                var best = eligible
                    .OrderByDescending(r => r.Priority)           // highest priority wins
                    .ThenByDescending(r => r.BossHealthPercent)   // tie-breaker
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
            if (!IsAuthorizedFromHeader(Request.Headers))
                return Unauthorized(new { message = "Invalid admin hash." });

            var result = _store.GetAll()
                .Select(kv => kv.Value)
                .OrderBy(r => r.Priority)
                .ThenBy(r => r.Name)
                .Select(RankDto.From)
                .ToArray();
            return Ok(result);
        }

        // GET: api/rolecheck/ranks/{id}
        [HttpGet("ranks/{id}")]
        public ActionResult<RankDto> GetById([FromRoute] string id)
        {
            if (!IsAuthorizedFromHeader(Request.Headers))
                return Unauthorized(new { message = "Invalid admin hash." });

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
            if (!IsAuthorizedFromHeader(Request.Headers))
                return Unauthorized(new { message = "Invalid admin hash." });

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
                InstanceType = request.InstanceType,
                BossBuffName = string.IsNullOrWhiteSpace(request.BossBuffName) ? null : request.BossBuffName.Trim(),
                BossBuffStackThreshold = Math.Max(0, request.BossBuffStackThreshold),
                Priority = request.Priority
            };

            _store.Set(request.DiscordRoleId, rank);

            var dto = RankDto.From(rank);
            return CreatedAtAction(nameof(GetById), new { id = request.DiscordRoleId.ToString() }, dto);
        }

        // PUT: api/rolecheck/ranks/{id}
        [HttpPut("ranks/{id}")]
        public IActionResult Update([FromRoute] string id, [FromBody] UpdateRankRequest request)
        {
            if (!IsAuthorizedFromHeader(Request.Headers))
                return Unauthorized(new { message = "Invalid admin hash." });

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
            existing.BossBuffName = string.IsNullOrWhiteSpace(request.BossBuffName) ? null : request.BossBuffName.Trim();
            existing.BossBuffStackThreshold = Math.Max(0, request.BossBuffStackThreshold);
            existing.Priority = request.Priority;

            _store.Set(key, existing); // persist

            return NoContent();
        }

        // DELETE: api/rolecheck/ranks/{id}
        [HttpDelete("ranks/{id}")]
        public IActionResult Delete([FromRoute] string id)
        {
            if (!IsAuthorizedFromHeader(Request.Headers))
                return Unauthorized(new { message = "Invalid admin hash." });

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

            // If a phase is configured, it must exist (supports "Name|start|duration" as well)
            var hasPhase = log.GetPhases(Array.Empty<string>())
                .Any(p => string.Equals(p.Split('|')[0].Split(':')[0], rank.Phase, StringComparison.Ordinal));
            if (!hasPhase)
                return false;

            // If configured, compute the time the boss reaches the required HP% in the phase
            long? reachedAtMs = null;
            if (rank.BossHealthPercent > 0)
            {
                var reachedAt = log.GetTimeForBossHealth(rank.Phase, rank.BossHealthPercent);
                if (reachedAt < 0)
                    return false;

                reachedAtMs = Convert.ToInt64(Math.Round(reachedAt, MidpointRounding.AwayFromZero));
            }

            // Optional: boss buff stack requirement at the time the boss reaches the required HP%
            if (rank.BossBuffStackThreshold > 0)
            {
                if (string.IsNullOrWhiteSpace(rank.BossBuffName))
                    return false;

                if (!reachedAtMs.HasValue)
                    return false;

                var start = log.GetPhaseStart(rank.Phase);
                var end = log.GetPhaseEnd(rank.Phase);
                var t = reachedAtMs.Value;

                if (t < start || t > end)
                    return false;

                var targets = log.GetTargets(rank.Phase);
                if (targets == null || targets.Length == 0)
                    return false;

                var anyTargetHasStacks = targets.Any(target =>
                    log.GetBoonAtTime(target, rank.BossBuffName!, t) >= rank.BossBuffStackThreshold);

                if (!anyTargetHasStacks)
                    return false;
            }

            return true;
        }

        // Instance detection from the log's instance name (exact matches only)
        private static InstanceType DetectInstanceType(IParsedEvtcLog log)
        {
            var name = log.GetInstanceName()?.Trim();
            if (string.IsNullOrEmpty(name))
                return InstanceType.Unknown;

            var map = new Dictionary<string, InstanceType>(StringComparer.OrdinalIgnoreCase)
            {
                ["Harvest Temple CM"] = InstanceType.HTCM,
                ["Temple of Febe CM"] = InstanceType.ToFCM,
                ["Greer CM"] = InstanceType.GreerCM,
                ["Decima CM"] = InstanceType.DecimaCM,
                ["Ura CM"] = InstanceType.UraCM,
                // Legendary/LCM aliases (if applicable in your logs)
                ["Ura LCM"] = InstanceType.UraLM,
                ["Ura Legendary CM"] = InstanceType.UraLM,
                ["Ura Legendary Mode"] = InstanceType.UraLM
            };

            return map.TryGetValue(name, out var it) ? it : InstanceType.Unknown;
        }

        private static string[] SafeArray(string[]? arr) => arr ?? Array.Empty<string>();

        private static bool ContainsAny(IEnumerable<string> haystack, params string[] needles)
        {
            foreach (var s in haystack)
            {
                foreach (var n in needles)
                {
                    if (s?.IndexOf(n, StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                }
            }
            return false;
        }

        // Parse helpers
        private static IParsedEvtcLog ParseLog(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();

            if (ext == ".json")
            {
                var jsonParser = new JsonParser();
                return jsonParser.ParseLog(path);
            }

            var libraryParser = new LibraryParser(false);
            return libraryParser.ParseLog(path);
        }

        // Download helpers (preserve extension if possible)
        private static async Task<string> DownloadToTempAsync(string url, CancellationToken ct)
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            // Prefer JSON extension if response is JSON (so ParseLog picks JsonParser)
            var media = response.Content.Headers.ContentType?.MediaType;
            string ext;
            if (!string.IsNullOrWhiteSpace(media) && media.Contains("json", StringComparison.OrdinalIgnoreCase))
            {
                ext = ".json";
            }
            else
            {
                var fileName = GetFileNameFromResponse(url, response);
                ext = Path.GetExtension(fileName);
                if (string.IsNullOrWhiteSpace(ext))
                    ext = ".zevtc"; // default fallback
            }

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
            try
            {
                var uri = new Uri(url);
                var last = Path.GetFileName(uri.LocalPath);
                if (!string.IsNullOrWhiteSpace(last))
                    return last;
            }
            catch { /* ignore */ }
            return "log.zevtc";
        }

        // dps.report helpers
        private static string NormalizeLogUrl(string url)
        {
            // Convert dps.report permalinks or getUpload links to getJson so we can parse JSON
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return url;

            if (!uri.Host.EndsWith("dps.report", StringComparison.OrdinalIgnoreCase))
                return url;

            // If already using getJson, keep as is
            if (uri.AbsolutePath.Equals("/getJson", StringComparison.OrdinalIgnoreCase))
                return url;

            // If a permalink is in the query, reuse it; otherwise, take the last path segment as permalink
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var permalink = query.Get("permalink");
            if (string.IsNullOrWhiteSpace(permalink))
            {
                var lastSegment = uri.Segments.LastOrDefault()?.Trim('/');
                if (!string.IsNullOrWhiteSpace(lastSegment))
                    permalink = lastSegment;
            }

            if (string.IsNullOrWhiteSpace(permalink))
                return url;

            var normalized = $"{uri.Scheme}://{uri.Host}/getJson?permalink={Uri.EscapeDataString(permalink)}";
            return normalized;
        }

        // Auth helpers
        private static bool IsAuthorizedFromHeader(IHeaderDictionary headers)
        {
            if (!headers.TryGetValue("X-Admin-Auth", out var provided)) return false;
            var providedHex = provided.ToString().Trim().ToLowerInvariant();
            var serverHashHex = ComputeSha256Hex(AdminPasswordPlaintext).ToLowerInvariant();

            // Constant-time compare
            var a = Encoding.ASCII.GetBytes(serverHashHex);
            var b = Encoding.ASCII.GetBytes(providedHex);
            return a.Length == b.Length && CryptographicOperations.FixedTimeEquals(a, b);
        }

        private static string ComputeSha256Hex(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = SHA256.HashData(bytes);
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        // DTOs (updated)
        public sealed record RankDto(
            ulong DiscordRoleId,
            string Name,
            string Phase,
            double BossHealthPercent,
            InstanceType InstanceType,
            int Priority)
        {
            public static RankDto From(Rank r) =>
                new(r.DiscordRoleId, r.Name, r.Phase, r.BossHealthPercent, r.InstanceType, r.Priority);
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
            public string? BossBuffName { get; set; }
            [Range(0, 100)]
            public int BossBuffStackThreshold { get; set; }
            [Range(-10000, 10000)]
            public int Priority { get; set; } = 0;
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
            public string? BossBuffName { get; set; }
            [Range(0, 100)]
            public int BossBuffStackThreshold { get; set; }
            [Range(-10000, 10000)]
            public int Priority { get; set; } = 0;
        }
    }
}
