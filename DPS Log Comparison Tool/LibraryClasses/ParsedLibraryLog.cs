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
using GW2EIEvtcParser.ParsedData;

namespace Bulk_Log_Comparison_Tool.LibraryClasses
{
    internal class ParsedLibraryLog : IParsedEvtcLog
    {
        private ParsedEvtcLog _log;
        private string _path;
        private Dictionary<string, (long, long)> _customPhases = new();

        public ParsedLibraryLog(ParsedEvtcLog log, string path)
        {
            _log = log;
            _path = path;
        }

        public string GetFileName()
        {
            return _path;
        }

        public void AddPhase(string name, long start, long duration)
        {
            if(!_customPhases.ContainsKey(name))
            {
                _customPhases.Add(name, (start, duration));
            }
        }

        public string GetLogStart()
        {
            return _log.LogData.LogStart;
        }

        public string[] GetPhases()
        {
            return _log.FightData.GetPhases(_log).Select(x => x.Name).Concat(_customPhases.Select(x => $"{x.Key}|{x.Value.Item1}|{x.Value.Item2}")).ToArray();
        }

        public string[] GetTargets(string phaseName = "")
        {
            var phase = GetPhaseFromName(phaseName);
            if (phase.Item1 == null)
                return [""];
            return phase.Item1.Targets.Select(x => x.Character).ToArray();
        }

        public int GetPlayerDps(string accountName, string phaseName = "", DamageTyping damageType = DamageTyping.All)
        {
            var phase = GetPhaseFromName(phaseName);
            if (phase.Item1 == null)
                return 0;
            var target = phase.Item1.Targets;
            if (target == null)
                return 0;
            return GetPlayerDps(accountName, GetPhaseStart(phaseName), GetPhaseEnd(phaseName), target.Select(x => x.Character).ToArray(), damageType);
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
            if (phase.Item1 == null)
                return 0;
            return GetBoon(target, boonName, GetPhaseStart(phaseName), GetPhaseEnd(phaseName));
        }

        public double GetBoon(string target, string boonName, long start, long end)
        { 
            AbstractSingleActor? Target = _log.PlayerList.FirstOrDefault(x => x.Account == target);
            if (Target == null)
            {
                Target = _log.FightData.GetMainTargets(_log).FirstOrDefault(x => x.Equals("target"));
                if(Target == null)
                {
                    return 0;
                }
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
            if (phase.Item1 == null)
                return 0;
            return GetBoon(group, boonName, GetPhaseStart(phaseName), GetPhaseEnd(phaseName));
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
            var revealedRemovedEvent = _log.CombatData.GetBuffRemoveAllData(890);

            foreach (var Invis in MassInvis)
            {
                var phases = _log.FightData.GetPhases(_log);
                var PlayersStealthed = new List<string>();
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

                List<GW2EIEvtcParser.ParsedData.BuffRemoveAllEvent> RemovedStealthEvents = _log.CombatData.GetBuffRemoveAllData(10269).Where(x => x.Time >= Invis.Time && x.Time <= Invis.Time + 20000).ToList();
                List<GW2EIEvtcParser.ParsedData.BuffRemoveAllEvent> RemovedRevealedEvents = _log.CombatData.GetBuffRemoveAllData(890).Where(x => x.Time >= Invis.Time && x.Time <= Invis.Time + 20000).ToList();
                if (RemovedStealthEvents.Count == 0)
                {
                    continue;
                }
                if(RemovedRevealedEvents.Where(x => x.To.Name.Contains(accountName)).Count() == 0)
                {
                    StealthResult.Add((FromPhase.Name, "✓"));
                    continue;
                }
                var median = RemovedStealthEvents[(int)Math.Floor(RemovedStealthEvents.Count / 2f)];
                foreach (var RemovedEvent in RemovedRevealedEvents.Where(x => x.To.Name.Contains(accountName)))
                {
                    //Check for revealed debuff instead
                    var error = "";
                    var player = _log.PlayerAgents.Where(x => x.Name.Contains(accountName)).FirstOrDefault();
                    if (player != null)
                    {
                        var deaths = _log.CombatData.GetDeadEvents(player);
                        foreach (var death in deaths)
                        {
                            if (death.Time >= Invis.EndTime && death.Time <= Invis.EndTime + 10000)
                            {
                                error = $"Died";
                                break;
                            }
                        }
                    }
                    var RevealedTime = (RemovedEvent.Time + RemovedEvent.RemovedDuration - 3000f);
                    if (median.Time - RevealedTime > 1000f)
                    {
                        var dmgData = _log.CombatData.GetDamageData(RemovedEvent.To).Where(x => x.Time >= Invis.EndTime && x.Time <= RemovedEvent.Time+10000).OrderBy(x => x.Time);
                        var skill = dmgData.FirstOrDefault(x => !x.Skill.Name.Equals("Nourishment") && x is DirectHealthDamageEvent);
                        if(skill == null)
                        {
                            skill = dmgData.FirstOrDefault(x => !x.Skill.Name.Equals("Nourishment"));
                        }
                        if (skill == null)
                        {
                            if (error == "")
                            {
                                error = "Unknown";
                            }
                        }
                        else
                        {
                            error = $"{-(int)(median.Time - RevealedTime) / 1000f}s { skill.Skill.Name}";
                        }
                    }
                    if (error != "")
                    {
                        StealthResult.Add((FromPhase.Name, error));
                    }
                    else
                    {
                        StealthResult.Add((FromPhase.Name, "✓"));
                    }
                }
            }
            return StealthResult;
        }


        private (PhaseData?,long,long) GetPhaseFromName(string phaseName)
        {
            PhaseData? phase = null;
            long start = 0;
            long duration = 0;
            if (phaseName == "")
            {
                phase = _log.FightData.GetPhases(_log).FirstOrDefault();
            }
            else
            {
                var splitPhaseName = phaseName.Split('|');
                if(splitPhaseName.Length == 3)
                {
                    phaseName = splitPhaseName[0].Split(':').First();
                }
                phase = _log.FightData.GetPhases(_log).Where(x => x.Name == phaseName).FirstOrDefault();
                if(phase == null)
                {
                    return (null, 0, 0);
                }
                start = phase.Start;
                duration = phase.End - phase.Start;
                if (splitPhaseName.Length == 3)
                {
                    start = start+long.Parse(splitPhaseName[1]) * 1000;
                    duration = long.Parse(splitPhaseName[2]) * 1000;
                }
            }
            return (phase,start, duration);
        }

        public string[] GetPlayers()
        {
            return _log.PlayerList.Select(x => x.Account).Distinct().ToArray();
        }
        
        public int[] GetGroups()
        {
            return _log.PlayerList.Select(x => x.Group).Distinct().ToArray();
        }

        public IEnumerable<string> GetBoonNames()
        {
            return _log.StatisticsHelper.PresentBoons.Select(x => x.Name);
        }

        public long GetPhaseStart(string phase)
        {
            var Phase = GetPhaseFromName(phase);
            var start = Phase.Item2 == 0 ? Phase.Item1?.Start ?? 0 : Phase.Item2;
            return start;
        }
        public long GetPhaseEnd(string phase)
        {
            var Phase = GetPhaseFromName(phase);
            var start = Phase.Item2 == 0 ? Phase.Item1?.Start ?? 0 : Phase.Item2;
            var end = start + Phase.Item3;
            if(end == start)
            {
                end = Phase.Item1?.End ?? 0;
            }
            return end;
        }

        public bool HasBoonDuringTime(string target, string boonName, long start, long end)
        {
            AbstractSingleActor? Target = _log.PlayerList.FirstOrDefault(x => x.Account == target);


            var Buffs = Target?.GetBuffs(BuffEnum.Self, _log, start, start+1) ?? null;
            foreach (var Boon in _log.StatisticsHelper.PresentBoons.Where(x => x.Name.Equals(boonName, StringComparison.OrdinalIgnoreCase)))
            {
                if (Buffs?.TryGetValue(Boon.ID, out var value) ?? false && value != null)
                {
                    if(value.Uptime == 0)
                    {
                        return false;
                    }
                    List<GW2EIEvtcParser.ParsedData.BuffRemoveAllEvent> RemovedStealthEvents = _log.CombatData.GetBuffRemoveAllData(Boon.ID).Where(x => x.RemovedDuration == 0 && x.To.Name.Contains(target) && x.Time >= start && x.Time <= end).ToList();
                    if (RemovedStealthEvents.Count > 0)
                    {
                        var stabLeft = value.Uptime - RemovedStealthEvents.Sum(x => x.RemovedStacks);
                        if(stabLeft == 0)
                        {
                            return false; //lost boon during period
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public bool IsAlive(string player, long time)
        {
            AbstractSingleActor? Target = _log.PlayerList.FirstOrDefault(x => x.Account == player);
            return !Target?.IsDead(_log, time) ?? false;
        }
    }
}
