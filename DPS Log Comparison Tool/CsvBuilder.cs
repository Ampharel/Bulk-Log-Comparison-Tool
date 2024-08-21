using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Numerics;
using Bulk_Log_Comparison_Tool.DataClasses;
using GW2EIEvtcParser.EIData;

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

            foreach (var phaseName in logs.GetPhases())
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
                var totalDps = new Dictionary<string, int>();
                foreach(var Player in Players)
                {
                    List<int> playerDps = new();
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
                        sb.Append($"{TrimmedAverage(playerDps).Average()},");
                    }
                    sb.Append($"\n");
                }
                sb.Append("Total,");
                List<int> totalDpsNumbers = new();
                foreach (var dpsKvp in totalDps)
                {
                    totalDpsNumbers.Add(dpsKvp.Value);
                    sb.Append(dpsKvp.Value + ",");
                }
                sb.Append($",");
                sb.Append($"{TrimmedAverage(totalDpsNumbers).Average()}");

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

            //foreach (var PlayerInfo in PlayerNames)
            //{
            //    WritePlayerLog(FightLogs.ToList(), OutputDirectory, PhaseNames, PlayerInfo);
            //}
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
                    var StealthResult = logs.GetStealthResult(log.GetFileName(), phase);
                    sb.Append($"{StealthResult},");
                }
                sb.Append("\n");
            }
        }

        //private static void WritePlayerLog(List<ParsedEvtcLog> logs, string OutputDirectory, List<PhaseInformation> PhaseNames, (string Account, Spec Spec, int Group) PlayerInfo)
        //{
        //    StringBuilder playerDps = new StringBuilder();
        //    foreach (var phaseName in PhaseNames)
        //    {
        //        List<IReadOnlyList<int>> DamageList = new();
        //        foreach (var log in logs)
        //        {
        //            var phase = log.FightData.GetPhases(log).Where(x => x.Name == phaseName.Name).FirstOrDefault();
        //            if (phase == null) continue;
        //            var player = log.PlayerList.FirstOrDefault(x => x.Account == PlayerInfo.Account);
        //            if (player == null) continue;
        //            var target = phase.Targets.OfType<NPC>().FirstOrDefault();

        //            if (phaseName.EndTime == 0)
        //            {
        //                DamageList.Add(player.Get1SDamageList(log, phase.Start, phase.End, target, DamageType.All));
        //            }
        //            else
        //            {
        //                DamageList.Add(player.Get1SDamageList(log, phase.Start + phaseName.StartTime, phase.Start + phaseName.EndTime, target, DamageType.All));
        //            }
        //        }
        //        playerDps.Append($"{phaseName}\n");

        //        var Max = 0;
        //        if (DamageList.Count > 0)
        //        {
        //            Max = DamageList.Select(x => x.Count).Max();
        //        }
        //        for (int i = 0; i < Max; i++)
        //        {
        //            playerDps.Append($"{i},");
        //            for (int j = 0; j < DamageList.Count; j++)
        //            {
        //                if (i < DamageList[j].Count)
        //                {
        //                    playerDps.Append($"{DamageList[j][i]},");
        //                }
        //                else
        //                {
        //                    playerDps.Append($",");
        //                }
        //            }
        //            playerDps.Append($"\n");
        //        }
        //        playerDps.Append($"\n");
        //    }
        //    Console.WriteLine($"Saving file {OutputDirectory}/{PlayerInfo.Account}.csv");
        //    File.WriteAllText($"{OutputDirectory}/{PlayerInfo.Account}.csv", playerDps.ToString());
        //}

        //public void WriteBoonsForTarget(ref StringBuilder sb, List<ParsedEvtcLog> logs, PhaseInformation phase)
        //{
        //    List<Buff> bossBuffs = new();
        //    foreach (var log in logs)
        //    {
        //        var stats = log.StatisticsHelper;
        //        var bossPhase = log.FightData.GetPhases(log).Where(x => x.Name == phase.Name).FirstOrDefault();
        //        if(bossPhase == null)
        //        {
        //            continue;
        //        }

        //        var start = phase.StartTime;
        //        var end = phase.EndTime;
        //        if(start == 0)
        //        {
        //            start = bossPhase.Start;
        //        }
        //        if (phase.EndTime != 0)
        //        {
        //            end = phase.StartTime + phase.EndTime;
        //        }
        //        else
        //        {
        //            end = bossPhase.End;
        //        }
        //        if(bossPhase == null)
        //        {
        //            continue;
        //        }
        //        var boss = bossPhase.Targets.OfType<NPC>().FirstOrDefault();
        //        if (boss == null)
        //        {
        //            continue;
        //        }
        //        IReadOnlyDictionary<long, FinalActorBuffs> bossBoons = boss.GetBuffs(BuffEnum.Self, log, start, end);
        //        foreach (var Boon in stats.PresentBoons)
        //        {
        //            bossBoons.TryGetValue(Boon.ID, out var value);
        //            if (value != null)
        //            {
        //                if(!bossBuffs.Exists(x => x.ID == Boon.ID))
        //                {
        //                    if (value.Uptime > 0)
        //                    {
        //                        bossBuffs.Add(Boon);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    //sb.Append($"phaseName {BoonName}");

        //    foreach (var buff in bossBuffs)
        //    {
        //        sb.Append($"Boss {buff.Name},");
        //        foreach (var log in logs)
        //        {
        //            var bossPhase = log.FightData.GetPhases(log).Where(x => x.Name == phase.Name).FirstOrDefault();
        //            if (bossPhase == null)
        //            {
        //                sb.Append($",");
        //                continue;
        //            }
        //            var boss = bossPhase.Targets.OfType<NPC>().FirstOrDefault();
        //            if (boss == null)
        //            {
        //                sb.Append($",");
        //                continue;
        //            }
        //            var start = phase.StartTime;
        //            var end = phase.EndTime;
        //            if (start == 0)
        //            {
        //                start = bossPhase.Start;
        //            }
        //            if (phase.EndTime != 0)
        //            {
        //                end = phase.StartTime + phase.EndTime;
        //            }
        //            else
        //            {
        //                end = bossPhase.End;
        //            }

        //            IReadOnlyDictionary<long, FinalActorBuffs> bossBoons = boss.GetBuffs(BuffEnum.Self, log, start, end);
        //            bossBoons.TryGetValue(buff.ID, out var value);
        //            if(value != null)
        //            {
        //                sb.Append($"{Math.Round(value.Uptime, 1)}%,");
        //            }
        //            else
        //            {
        //                sb.Append($",");
        //            }
        //        }
        //        sb.Append($"\n");
        //    }
        //    sb.Append($"\n");
        //}
        //public void WriteBoons(ref StringBuilder sb, int Group, string BoonName, List<ParsedEvtcLog> logs, PhaseInformation phaseName, bool PlayersAlive = false)
        //{
        //    sb.Append($"G{Group} {BoonName}");
        //    foreach (var log in logs)
        //    {
        //        var phase = log.FightData.GetPhases(log).Where(x => x.Name == phaseName.Name).FirstOrDefault();
        //        if (phase == null)
        //        {
        //            sb.Append($",");
        //            continue;
        //        }
        //        var Players = log.PlayerList.Where(x => x.Group == Group).ToList();
        //        if (PlayersAlive)
        //        {
        //            if (phaseName.EndTime == 0)
        //            {
        //                Players = Players.Where(x => !x.IsDead(log, phase.Start, phase.End)).ToList();
        //            }
        //            else
        //            {
        //                Players = Players.Where(x => !x.IsDead(log, phase.Start + phaseName.StartTime, phase.Start + phaseName.EndTime)).ToList();
        //            }
        //        }
        //        if (Players.Count == 0)
        //        {
        //            sb.Append($",");
        //            continue;
        //        }
        //        var TotalBoonUptime = 0d;
        //        var BoonType = Buff.BuffType.Duration;
        //        var stats = log.StatisticsHelper;
        //        foreach (var Player in Players)
        //        {
        //            IReadOnlyDictionary<long, FinalActorBuffs> PlayerBoons;
        //            if (phaseName.EndTime == 0)
        //            {
        //                PlayerBoons = Player.GetBuffs(BuffEnum.Self, log, phase.Start, phase.End);
        //            }
        //            else
        //            {
        //                PlayerBoons = Player.GetBuffs(BuffEnum.Self, log, phase.Start + phaseName.StartTime, phase.Start + phaseName.EndTime);
        //            }
        //            foreach (var Boon in stats.PresentBoons)
        //            {
        //                if (Boon.Name == BoonName.Split(' ').FirstOrDefault())
        //                {
        //                    PlayerBoons.TryGetValue(Boon.ID, out var value);
        //                    if (value != null)
        //                    {
        //                        BoonType = Boon.Type;
        //                        TotalBoonUptime += value.Uptime;
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //        var Uptime = Math.Round(TotalBoonUptime / Players.Count, 1);
        //        if (BoonType == Buff.BuffType.Duration)
        //        {
        //            sb.Append($",{Uptime}%");
        //        }
        //        else if (BoonType == Buff.BuffType.Intensity)
        //        {
        //            sb.Append($",{Uptime}");
        //        }
        //    }
        //    sb.Append($"\n");
        //}

        //private void WritePhase(PhaseInformation phaseData, ref StringBuilder sb, List<ParsedEvtcLog> logs, List<(string Account, Spec Spec, int Group)> PlayerNames)
        //{
        //    sb.Append($"{phaseData.Name}\n");
        //    sb.Append($"{phaseData.Description}\n");
        //    for (int i = 0; i < logs.Count; i++)
        //    {
        //        sb.Append($",");
        //    }
        //    sb.Append(",Trimmed mean");
        //    sb.Append($"\n");
        //    foreach (var PlayerInfo in PlayerNames)
        //    {
        //        sb.Append($"{PlayerInfo.Group}. {PlayerInfo.Account} [{PlayerInfo.Spec}]");

        //        Console.WriteLine($"Writing phase {phaseData} for player {PlayerInfo.Account}");
        //        var dpsList = new List<int>();
        //        foreach (var log in logs)
        //        {
        //            var phase = log.FightData.GetPhases(log).Where(x => x.Name == phaseData.Name).FirstOrDefault();
        //            if (phase == null)
        //            {
        //                sb.Append($",");
        //                continue;
        //            }
        //            var target = phase.Targets.OfType<NPC>().FirstOrDefault();
        //            var Player = log.PlayerList.FirstOrDefault(x => x.Account == PlayerInfo.Account && x.Spec == PlayerInfo.Spec);
        //            if (Player == null)
        //            {
        //                sb.Append($",");
        //                continue;
        //            }
        //            FinalDPS? dpsStats = null;
        //            if (phaseData.EndTime == 0)
        //            {
        //                dpsStats = Player.GetDPSStats(target, log, phase.Start, phase.End);
        //            }
        //            else
        //            {
        //                dpsStats = Player.GetDPSStats(target, log, phase.Start + phaseData.StartTime, phase.Start + phaseData.EndTime);
        //            }
        //            dpsList.Add(dpsStats.Dps);

        //            if(dpsStats.Dps > 5000)
        //            {
        //                sb.Append($",{Math.Round(dpsStats.Dps / 1000f, 1)}k");
        //            }
        //            else
        //            {
        //                sb.Append($",");
        //            }
        //        }
        //        dpsList = TrimmedAverage(dpsList);
        //        if (dpsList.Count > 0)
        //        {
        //            sb.Append($",{Math.Round(dpsList.Average() / 1000f, 1)}k");
        //        }
        //        sb.Append($"\n");
        //    }
        //    Console.WriteLine($"Writing total dps phase {phaseData}");
        //    sb.Append($"Total");

        //    var totalDpsList = new List<int>();
        //    foreach (var log in logs)
        //    {
        //        var phase = log.FightData.GetPhases(log).Where(x => x.Name == phaseData.Name).FirstOrDefault();
        //        if (phase == null)
        //        {
        //            sb.Append($",");
        //            continue;
        //        }
        //        var target = phase.Targets.OfType<NPC>().FirstOrDefault();
        //        List<int> dpsStats;
        //        if (phaseData.EndTime == 0)
        //        {
        //            dpsStats = log.PlayerList.Select(x => x.GetDPSStats(target, log, phase.Start, phase.End).Dps).ToList();
        //        }
        //        else
        //        {
        //            dpsStats = log.PlayerList.Select(x => x.GetDPSStats(target, log, phase.Start + phaseData.StartTime, phase.Start + phaseData.EndTime).Dps).ToList();
        //        }
        //        var sum = dpsStats.Sum();
        //        totalDpsList.Add(sum);
        //        sb.Append($",{Math.Round(sum / 1000f, 1)}k");
        //    }
        //    totalDpsList = TrimmedAverage(totalDpsList);
        //    if (totalDpsList.Count > 0)
        //    {
        //        sb.Append($",{Math.Round(totalDpsList.Average() / 1000f, 1)}k");
        //    }
        //    else
        //    {
        //        sb.Append($",");
        //    }

        //    sb.Append($"\n\n");

        //    Console.WriteLine($"Writing boons for phase {phaseData}");
        //    var Groups = logs.SelectMany(x => x.PlayerList.Select(x => x.Group)).Distinct().ToList();
        //    foreach (int Group in Groups)
        //    {
        //        WriteBoons(ref sb, Group, "Might", logs, phaseData);
        //    }
        //    foreach (int Group in Groups)
        //    {
        //        WriteBoons(ref sb, Group, "Quickness", logs, phaseData);
        //    }
        //    foreach (int Group in Groups)
        //    {
        //        WriteBoons(ref sb, Group, "Alacrity", logs, phaseData);
        //    }

        //    sb.Append($"\n");
        //    Console.WriteLine($"Writing alive only boons for phase {phaseData}");
        //    foreach (int Group in Groups)
        //    {
        //        WriteBoons(ref sb, Group, "Might Alive", logs, phaseData, true);
        //    }
        //    foreach (int Group in Groups)
        //    {
        //        WriteBoons(ref sb, Group, "Quickness Alive", logs, phaseData, true);
        //    }
        //    foreach (int Group in Groups)
        //    {
        //        WriteBoons(ref sb, Group, "Alacrity Alive", logs, phaseData, true);
        //    }

        //    sb.Append($"\n");

        //    Console.WriteLine($"Writing target boons for phase {phaseData}");
        //    WriteBoonsForTarget(ref sb, logs, phaseData);
        //    sb.Append($"\n");
        //}

        //private void WritePhaseDataForPlayer(PhaseInformation phaseData, ref StringBuilder sb, List<ParsedEvtcLog> logs, (string Account, Spec Spec, int Group) PlayerInfo)
        //{
        //    sb.Append($"{PlayerInfo.Group}. {PlayerInfo.Account} [{PlayerInfo.Spec}]");

        //    Console.WriteLine($"Writing phase {phaseData} for player {PlayerInfo.Account}");
        //    var dpsList = new List<int>();
        //    foreach (var log in logs)
        //    {
        //        var phase = log.FightData.GetPhases(log).Where(x => x.Name == phaseData.Name).FirstOrDefault();
        //        if (phase == null)
        //        {
        //            sb.Append($",");
        //            continue;
        //        }
        //        var target = phase.Targets.OfType<NPC>().FirstOrDefault();
        //        var Player = log.PlayerList.FirstOrDefault(x => x.Account == PlayerInfo.Account && x.Spec == PlayerInfo.Spec);
        //        if (Player == null)
        //        {
        //            sb.Append($",");
        //            continue;
        //        }
        //        FinalDPS? dpsStats = null;
        //        if (phaseData.EndTime == 0)
        //        {
        //            dpsStats = Player.GetDPSStats(target, log, phase.Start, phase.End);
        //        }
        //        else
        //        {
        //            dpsStats = Player.GetDPSStats(target, log, phase.Start + phaseData.StartTime, phase.Start + phaseData.EndTime);
        //        }
        //        dpsList.Add(dpsStats.Dps);

        //        sb.Append($",{Math.Round(dpsStats.Dps / 1000f, 1)}k");
        //    }
        //    dpsList = TrimmedAverage(dpsList);
        //    if (dpsList.Count > 0)
        //    {
        //        sb.Append($",{Math.Round(dpsList.Average() / 1000f, 1)}k");
        //    }
        //    sb.Append($"\n");
        //}

        public List<int> TrimmedAverage(List<int> ints)
        {
            if (ints.Count == 0)
            {
                return ints;
            }
            var sortedInts = ints.OrderBy(x => x).ToList();
            var Max = sortedInts.Max();
            return sortedInts.Where(x => x >= Max * 0.6).ToList();
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
