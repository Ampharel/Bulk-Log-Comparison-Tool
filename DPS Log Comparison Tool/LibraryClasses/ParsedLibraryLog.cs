using Bulk_Log_Comparison_Tool.DataClasses;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using Bulk_Log_Comparison_Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static GW2EIEvtcParser.ParserHelper;
using System.Collections;
using static GW2EIEvtcParser.ArcDPSEnums;
using Bulk_Log_Comparison_Tool.Util;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace Bulk_Log_Comparison_Tool.LibraryClasses
{
    internal class ParsedLibraryLog : IParsedEvtcLog
    {
        private ParsedEvtcLog _log;
        private string _path;

        public ParsedLibraryLog(ParsedEvtcLog log, string path)
        {
            _log = log;
            _path = path;
        }

        public string GetFileName()
        {
            return _path;
        }

        public string GetLogStart()
        {
            return _log.LogData.LogStart;
        }

        public string[] GetPhases()
        {
            return _log.FightData.GetPhases(_log).Select(x => x.Name).ToArray();
        }

        public string[] GetTargets(string phaseName = "")
        {
            var phase = GetPhaseFromName(phaseName);
            if (phase == null)
                return [""];
            return phase.Targets.Select(x => x.Character).ToArray();
        }

        public int GetPlayerDps(string accountName, string phaseName = "", DamageTyping damageType = DamageTyping.All)
        {
            var phase = GetPhaseFromName(phaseName);
            if (phase == null)
                return 0;
            var target = phase.Targets;
            if (target == null)
                return 0;
            return GetPlayerDps(accountName, phase.Start, phase.End, target.Select(x => x.Character).ToArray(), damageType);
        }

        public int GetPlayerDps(string accountName, long start, long end, string[] targetNames, DamageTyping damageType = DamageTyping.All)
        {
            var player = _log.PlayerList.FirstOrDefault(x => x.Account == accountName);
            if (player == null) return 0;
            var targets = _log.FightData.GetMainTargets(_log).Where(x => targetNames.Contains(x.Character));

            int dps = 0;
            foreach (var target in targets)
            {
                if (target == null) return 0;

                var DamageList = player.GetDPSStats(target, _log, start, end);


                if ((damageType & DamageTyping.Power) != 0)
                {
                    dps += DamageList.PowerDps;
                }
                if ((damageType & DamageTyping.Condition) != 0)
                {
                    dps += DamageList.CondiDps;
                }
                if ((damageType & DamageTyping.LifeLeech) != 0)
                {
                    dps += DamageList.LifeLeechDps;
                }
                if ((damageType & DamageTyping.Barrier) != 0)
                {
                    dps += DamageList.BarrierDps;
                }
            }

            return dps;
        }

        public BuffStackTyping GetBoonStackType(string boonName)
        {
            return ConvertStackTypeEnum(_log.StatisticsHelper.PresentBoons.Where(x => x.Name.Equals(boonName, StringComparison.OrdinalIgnoreCase)).Select(x => x.StackType).FirstOrDefault());
        }

        public double GetBoon(string target, string boonName, string phaseName = "")
        {
            var phase = GetPhaseFromName(phaseName);
            if (phase == null)
                return 0;
            return GetBoon(target, boonName, phase.Start, phase.End);
        }

        public double GetBoon(string target, string boonName, long start, long end)
        { 
            AbstractSingleActor? Target = _log.PlayerList.FirstOrDefault(x => x.Account == target);
            if (Target == null)
            {
                Target = _log.FightData.GetMainTargets(_log).First(x => x.Equals("target"));
            }

            var Buffs = Target.GetBuffs(BuffEnum.Self, _log, start, end);
            var targetBuffs = new List<Buff>();
            foreach (var Boon in _log.StatisticsHelper.PresentBoons.Where(x => x.Name.Equals(boonName, StringComparison.OrdinalIgnoreCase)))
            {
                Buffs.TryGetValue(Boon.ID, out var value);
                if (value != null)
                {
                    return value.Uptime;
                }
            }
            return 0;
        }

        public double GetBoon(int group, string boonName, string phaseName = "")
        {
            var phase = GetPhaseFromName(phaseName);
            if (phase == null)
                return 0;
            return GetBoon(group, boonName, phase.Start, phase.End);
        }

        public double GetBoon(int group, string boonName, long start, long end)
        {
            var groupMembers = _log.PlayerList.Where(x => x.Group == group);
            var boons = new List<double>();
            foreach (var Boon in _log.StatisticsHelper.PresentBoons.Where(x => x.Name.Equals(boonName, StringComparison.OrdinalIgnoreCase)))
            {
                foreach (var player in groupMembers)
                {
                    var Buffs = player.GetBuffs(BuffEnum.Self, _log, start, end);
                    Buffs.TryGetValue(Boon.ID, out var value);
                    if (value != null)
                    {
                        boons.Add(value.Uptime);
                    }
                }
            }
            return boons.Average();
        }

        private BuffStackTyping ConvertStackTypeEnum(BuffStackType type)
        {
            switch (type)
            {
                case BuffStackType.Stacking:
                    return BuffStackTyping.Stacking;
                case BuffStackType.Queue:
                    return BuffStackTyping.Queue;
                case BuffStackType.StackingConditionalLoss:
                    return BuffStackTyping.StackingConditionalLoss;
                case BuffStackType.StackingTargetUniqueSrc:
                    return BuffStackTyping.StackingTargetUniqueSrc;
                case BuffStackType.Regeneration:
                    return BuffStackTyping.Regeneration;
                case BuffStackType.Force:
                    return BuffStackTyping.Force;
                default:
                    return BuffStackTyping.Unknown;
            }
        }

        public List<(string, string)> GetStealthResult(string accountName)
        {
            var StealthResult = new List<(string, string)>();
            var MassInvis = _log.CombatData.GetAnimatedCastData(10245);
            var buffRemoved = _log.CombatData.GetBuffRemoveAllData(10269);
            var buffInfoEvent = _log.CombatData.GetBuffInfoEvent(10269);
            foreach (var Invis in MassInvis)
            {
                var phases = _log.FightData.GetPhases(_log);
                PhaseData? FromPhase = null;
                foreach (var ph in phases)
                {
                    if (ph.Start != 0 && ph.End > Invis.EndTime)
                    {
                        break;
                    }
                    FromPhase = ph;
                }
                if (FromPhase == null)
                {
                    continue;
                }
                List<GW2EIEvtcParser.ParsedData.BuffRemoveAllEvent> RemovedEvents = _log.CombatData.GetBuffRemoveAllData(10269).Where(x => x.Time >= Invis.EndTime && x.Time <= Invis.EndTime + 20000).ToList();
                if (RemovedEvents.Count == 0)
                {
                    continue;
                }
                double mean = RemovedEvents.Average(x => x.Time);
                var median = RemovedEvents[(int)Math.Floor(RemovedEvents.Count / 2f)];
                foreach (var RemovedEvent in RemovedEvents)
                {
                    var error = "";

                    if (RemovedEvent.To.Name.Contains(accountName) && median.Time - RemovedEvent.Time > 1000f)
                    {
                        var dmgData = _log.CombatData.GetDamageData(RemovedEvent.To).Where(x => x.Time > Invis.EndTime && x.Time < Invis.EndTime + 6000);
                        var StealthTime = RemovedEvent.Time - Invis.EndTime;
                        var skill = dmgData.FirstOrDefault(x => x.Skill.Name != "Nourishment");
                        if (skill == null)
                        {
                            error = "Unknown";
                        }
                        else
                        {
                            error = skill.Skill.Name;
                        }
                    }
                    if (error != "")
                    {
                        StealthResult.Add((FromPhase.Name, $"{-(int)(median.Time - RemovedEvent.Time) / 1000f}s {error}"));
                    }
                }
            }
            return StealthResult;
        }


        private PhaseData? GetPhaseFromName(string phaseName)
        {
            PhaseData? phase = null;
            if (phaseName == "")
            {
                phase = _log.FightData.GetPhases(_log).FirstOrDefault();
            }
            else
            {
                phase = _log.FightData.GetPhases(_log).Where(x => x.Name == phaseName).FirstOrDefault();
            }
            return phase;
        }

        public string[] GetPlayers()
        {
            return _log.PlayerList.Select(x => x.Account).Distinct().ToArray();
        }
        
        public int[] GetGroups()
        {
            return _log.PlayerList.Select(x => x.Group).Distinct().ToArray();
        }
    }
}
