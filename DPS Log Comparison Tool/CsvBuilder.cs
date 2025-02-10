using System.Text;
using Bulk_Log_Comparison_Tool.DataClasses;

namespace Bulk_Log_Comparison_Tool
{
    public class CsvBuilder()
    {

        private string[] FilteredPhases = { };
        private string[] SpecificPhase = { };
        public void CsvString(BulkLog logs, string OutputDirectory)
        {
            LoadFiles();
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            ParseLogs(logs, OutputDirectory);
        }

        private void LoadFiles()
        {
            if (!File.Exists("FilteredPhases.txt"))
            {
                Console.WriteLine("FilteredPhases.txt not found, creating new file");
                File.WriteAllText("FilteredPhases.txt", "#Add any phase name here, to exclude it from the generated report.\n#Lines starting with a # will be ignored");
            }
            if (!File.Exists("SpecificPhase.txt"))
            {
                Console.WriteLine("SpecificPhase.txt not found, creating new file");
                File.WriteAllText("SpecificPhase.txt", "#Add any phase name, appended with a ~ and the time.\n#Lines starting with a # will be ignored");
            }
            FilteredPhases = File.ReadAllLines("FilteredPhases.txt").Where(x => !x.StartsWith('#')).ToArray();
            SpecificPhase = File.ReadAllLines("SpecificPhase.txt").Where(x => !x.StartsWith('#')).ToArray();
        }

        public void ParseLogs(BulkLog logs, string OutputDirectory)
        {
            //var phasesPerLog = logs.Select(x => x.FightData.GetPhases(x)).ToList();
            //var allPhases = logs.Select(x => x.FightData.GetPhases(x)).SelectMany(x => x).OrderBy(x => x.Start).ToList();
            //var PhaseNames = allPhases.Select(y => new PhaseInformation { Name = y.Name }).Where(x => !FilteredPhases.Contains(x.Name)).DistinctBy(x => x.Name + x.EndTime).ToList();
            //PhaseNames.AddRange(SpecificPhase.Select(y => new PhaseInformation { Name = y.Split('~').FirstOrDefault() ?? "", Description = y.Split('~').ElementAtOrDefault(1) ?? "", StartTime = y.Split('~').ElementAtOrDefault(2).TryParse() * 1000, EndTime = y.Split('~').ElementAtOrDefault(3).TryParse() * 1000 }));
            //var PlayerNames = logs.Select(x => x.PlayerList).SelectMany(x => x).Select(x => (x.Account, x.Spec, x.Group)).OrderBy(x => x.Group).Distinct().ToList();

            Console.WriteLine("Writing report");
            StringBuilder sb = new StringBuilder();

            sb.Append("Log");

            foreach (var fileName in logs.Logs.Select(x => x.GetFileName()))
            {
                sb.Append("," + fileName);
            }

            sb.Append("\n");
            sb.Append("\n");

            WriteMechanics(ref sb, logs);

            sb.Append("\n");

            var Players = logs.GetPlayers();

            foreach (var phaseName in logs.GetPhases([]))
            {
                if (FilteredPhases.Contains(phaseName))
                {
                    continue;
                }
                sb.Append($"{phaseName}\n");

                for (int i = 0; i < logs.Logs.Count + 1; i++)
                {
                    sb.Append($",");
                }
                sb.Append(",Trimmed mean\n");
                var totalDps = new Dictionary<string, double>();
                foreach(var Player in Players)
                {
                    List<double> playerDps = new();
                    sb.Append($"{Player},");
                    foreach (var log in logs.Logs)
                    {
                        if(!totalDps.ContainsKey(log.GetFileName()))
                        {
                            totalDps[log.GetFileName()] = 0;
                        }
                        var dps = log.GetPlayerDps(Player, phaseName);
                        playerDps.Add(dps);
                        totalDps[log.GetFileName()] += dps;
                        sb.Append($"{dps},");
                    }

                    sb.Append($",");
                    if (playerDps.Count > 0)
                    {
                        sb.Append($"{Util.Util.TrimmedAverage(playerDps).Average()},");
                    }
                    sb.Append($"\n");
                }
                sb.Append("Total,");
                List<double> totalDpsNumbers = new();
                foreach (var dpsKvp in totalDps)
                {
                    totalDpsNumbers.Add(dpsKvp.Value);
                    sb.Append(dpsKvp.Value + ",");
                }
                sb.Append($",");
                sb.Append($"{Util.Util.TrimmedAverage(totalDpsNumbers).Average()}");

                sb.Append("\n");
                sb.Append("\n");

                var groups = logs.GetGroups();
                string[] boons = ["Might", "Quickness", "Alacrity"];

                foreach (var boon in boons)
                {
                    foreach (var group in groups)
                    {
                        sb.Append($"G{group} {boon},");
                        foreach (var log in logs.Logs)
                        {
                            var boonUptime = log.GetBoon(group, boon);
                            sb.Append($"{boonUptime},");
                        }
                        sb.Append("\n");
                    }
                }
                sb.Append("\n");
                sb.Append("\n");
            }


            if (sb.Length > 0)
            {
                if (!Directory.Exists(OutputDirectory))
                {
                    Directory.CreateDirectory(OutputDirectory);
                }
                Console.WriteLine($"Saving file {OutputDirectory}/result.csv");
                File.WriteAllText($"{OutputDirectory}/result.csv", sb.ToString());
            }
        }

        private void WriteMechanics(ref StringBuilder sb, BulkLog logs)
        {

            sb.Append($"Stealth anayltics:\n");
            string[] phases = logs.GetStealthPhases();
            foreach(var phase in phases)
            {
                sb.Append($"{phase},");
                foreach(var log in logs.Logs)
                {
                    var StealthResult = logs.GetStealthResult(log.GetFileName(), phase, Enums.StealthAlgoritmns.Timing);
                    sb.Append($"{StealthResult},");
                }
                sb.Append("\n");
            }
        }
    }

    public class PhaseInformation
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public long StartTime { get; set; } = 0;
        public long EndTime { get; set; } = 0;

        public override string ToString()
        {
            return Name;
        }
    }
}
