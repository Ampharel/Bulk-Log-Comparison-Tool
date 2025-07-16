using Bulk_Log_Comparison_Tool.DataClasses;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ArcDPSEnums;
using Bulk_Log_Comparison_Tool.Util;
using GW2EIEvtcParser.ParsedData;
using System;
using System.Numerics;
using Bulk_Log_Comparison_Tool.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;
using System.Xml.Linq;
using System.Globalization;
using System.Diagnostics.Metrics;

namespace Bulk_Log_Comparison_Tool.LibraryClasses
{
    internal class ParsedLibraryLog : IParsedEvtcLog
    {
        private ParsedEvtcLog _log;
        private string _path;
        private Dictionary<string, (long, long)> _customPhases = new();
        private Dictionary<string, string>? _expectedStealthPhases = new();



        public ParsedLibraryLog(ParsedEvtcLog log, string path)
        {
            _log = log;
            _path = path;


            LoadFile();
        }

        public string[] GetStealthPhases()
        {
            return _expectedStealthPhases.Keys.ToArray();
        }

        private void LoadFile()
        {
            _expectedStealthPhases = new();
            CreateFile();
            var stealthPhases = File.ReadAllLines("StealthPhases.txt").Where(x => !x.StartsWith("#") && x.Contains('|')).ToList();
            foreach (var phase in stealthPhases)
            {
                var splitPhase = phase.Split('|');
                var bossPhase = splitPhase.First();
                var description = splitPhase.Last();
                if (!_expectedStealthPhases.ContainsKey(bossPhase))
                {
                    _expectedStealthPhases.Add(bossPhase, description);
                }
            }
        }

        public string GetFileName()
        {
            return _path;
        }

        public void AddPhase(string name, long start, long duration)
        {
            if (!_customPhases.ContainsKey(name))
            {
                _customPhases.Add(name, (start, duration));
            }
        }

        public string GetLogStart()
        {
            return _log.LogData.LogStart;
        }

        public string[] GetPhases(string[] filter, bool exclusion = true)
        {
            var allPhases = _log.FightData.GetPhases(_log).Select(x => x.Name).Concat(_customPhases.Select(x => $"{x.Key}|{x.Value.Item1}|{x.Value.Item2}"));
            if (exclusion)
            {
                return allPhases.Where(x => filter.All(y => !x.Contains(y))).ToArray();
            }
            return allPhases.Where(x => filter.Any(y => x.Contains(y))).ToArray();
        }

        public string[] GetTargets(string phaseName = "")
        {
            var phase = GetPhaseFromName(phaseName);
            if (phase.Item1 == null)
                return [""];
            return phase.Item1.Targets.Select(x => x.Key.Character).ToArray();
        }

        public double GetPlayerDps(string accountName, string phaseName = "", bool allTarget = false, bool cumulative = false, bool defiance = false, DamageTyping damageType = DamageTyping.All)
        {
            var phase = GetPhaseFromName(phaseName);
            if (phase.Item1 == null)
                return 0;

            IReadOnlyList<SingleActor>? target = null;
            if (phaseName == "Full Fight" && allTarget)
            {
                target = _log.FightData.GetPhases(_log).SelectMany(x => x.Targets.Keys).DistinctBy(x => x.Character).ToArray();
            }
            else if (allTarget)
            {
                target = phase.Item1.Targets.Keys.ToArray();
            }
            else
            {
                target = [phase.Item1.Targets.Keys.First()];
            }
            if (target == null)
                return 0;
            return GetPlayerDps(accountName, GetPhaseStart(phaseName), GetPhaseEnd(phaseName), target.ToArray(), cumulative, defiance, damageType);
        }

        public double GetPlayerDps(string accountName, long start, long end, SingleActor[] targets, bool cumulative = false, bool defiance = false, DamageTyping damageType = DamageTyping.All)
        {
            var player = _log.PlayerList.FirstOrDefault(x => x.Account == accountName);
            if (player == null) return 0;
            if (targets == null)
            {
                return 0;
            }

            double dps = 0;
            foreach (var target in targets)
            {
                if (target == null) return 0;

                var DamageList = player.GetDamageStats(target, _log, start, end);

                if (defiance)
                {
                    dps += DamageList.BreakbarDamage;
                    continue;
                }

                if (!cumulative)
                {
                    if ((damageType & DamageTyping.Power) != 0)
                    {
                        dps += DamageList.PowerDPS;
                    }
                    if ((damageType & DamageTyping.Condition) != 0)
                    {
                        dps += DamageList.ConditionDPS;
                    }
                    if ((damageType & DamageTyping.LifeLeech) != 0)
                    {
                        dps += DamageList.LifeLeechDPS;
                    }
                    if ((damageType & DamageTyping.Barrier) != 0)
                    {
                        dps += DamageList.BarrierDPS;
                    }
                }
                else
                {
                    if ((damageType & DamageTyping.Power) != 0)
                    {
                        dps += DamageList.PowerDamage;
                    }
                    if ((damageType & DamageTyping.Condition) != 0)
                    {
                        dps += DamageList.ConditionDamage;
                    }
                    if ((damageType & DamageTyping.LifeLeech) != 0)
                    {
                        dps += DamageList.LifeLeechDamage;
                    }
                    if ((damageType & DamageTyping.Barrier) != 0)
                    {
                        dps += DamageList.BarrierDamage;
                    }
                }
            }

            return dps;
        }


        public double GetPlayerDps(string accountName, long time = 0, string phaseName = "",  bool allTarget = false, bool cumulative = false, bool defiance = false, DamageTyping damageType = DamageTyping.All)
        {
            //Maybe add burst time check
            var phase = GetPhaseFromName(phaseName);
            if (phase.Item1 == null)
                return 0;

            IReadOnlyList<SingleActor>? target = null;
            if (phaseName == "Full Fight" && allTarget)
            {
                target = _log.FightData.GetPhases(_log).SelectMany(x => x.Targets.Keys).DistinctBy(x => x.Character).ToArray();
            }
            else if(allTarget)
            {
                target = phase.Item1.Targets.Keys.ToArray();
            }
            else
            {
                target = [phase.Item1.Targets.Keys.First()];
            }
            if (target == null)
                return 0;
            var phaseStart = GetPhaseStart(phaseName);
            return GetPlayerDps(accountName, phaseStart, phaseStart+time, target.ToArray(), cumulative, defiance, damageType);
        }

        public BuffStackTyping GetBoonStackType(string boonName)
        {
            return ConvertStackTypeEnum(_log.StatisticsHelper.PresentBoons.Where(x => x.Name.Equals(boonName, StringComparison.OrdinalIgnoreCase)).Select(x => x.StackType).FirstOrDefault());
        }


        public double GetBoon(string target, string boonName, string phaseName = "", long time = 0, bool duration = false)
        {
            var phase = GetPhaseFromName(phaseName);
            if (phase.Item1 == null)
                return 0;
            var endTime = time == 0 ? GetPhaseEnd(phaseName) : GetPhaseStart(phaseName) + time;
            if (duration)
            {
                return GetBoonDurationInPhase(target, boonName, phaseName, time);
            }
            else
            {
                return GetBoon(target, boonName, GetPhaseStart(phaseName), endTime);
            }
        }

        public double GetBoonAtTime(string target, string boonName, long time)
        {
            return GetBoon(target, boonName, time, time+1);
        }

        public List<(double, double)> GetBoonTimedEvents(string target, string boonName, string phaseName = "", string source = "")
        {
            SingleActor? Target = _log.PlayerList.FirstOrDefault(x => x.Account == target);
            if (Target == null)
            {
                Target = _log.FightData.GetMainTargets(_log).FirstOrDefault(x => x.Equals(target));
                if (Target == null)
                {
                    return new();
                }
            }

            var boonStackingType = GetBoonStackType(boonName);
            switch(boonStackingType)
            {
                case BuffStackTyping.Stacking:
                case BuffStackTyping.StackingConditionalLoss:
                case BuffStackTyping.StackingTargetUniqueSrc:
                    return GetBoonStackingEvents([target], boonName, phaseName, source);
                case BuffStackTyping.Queue:
                case BuffStackTyping.Regeneration:
                    return GetBoonDurationEvents([target], boonName, phaseName, source);
                default:
                    return new();
            }
        }

        private List<(double, double)> GetBoonStackingEvents(string[] targets, string boonName, string phaseName, string source)
        {
            var startTime = GetPhaseStart(phaseName);
            var endTime = GetPhaseEnd(phaseName);
            var prevTime = startTime;

            var boonStart = 0d;
            foreach (var target in targets)
            {
                boonStart += GetBoon(target, boonName, startTime, startTime + 1);
            }
            boonStart /= targets.Length;
            var boonEvents = new List<(double, double)>
            {
                { (startTime, boonStart) }
            };
            foreach (var Boon in _log.StatisticsHelper.PresentBoons.Where(x => x.Name.Equals(boonName, StringComparison.OrdinalIgnoreCase)))
            {
                var _buffEvents = new List<BuffEvent>();
                foreach (var target in targets)
                {
                    _buffEvents.AddRange(GetBuffEventsForTarget(Boon.ID, target));
                }
                foreach(var buff in _buffEvents)
                {
                    if(buff.Time < startTime && startTime != 0)
                    {
                        continue;
                    }
                    if(buff.Time > endTime)
                    {
                        break;
                    }
                    if (boonEvents.Any(x => x.Item1 == buff.Time))
                    {
                        continue;
                    }
                    var buffApplyEvent = buff as BuffApplyEvent;
                    var buffExtensionEvent = buff as BuffExtensionEvent;
                    var buffRemovedEvent = buff as BuffRemoveAllEvent;
                    boonEvents.Add((buff.Time, boonStart));
                    var boonAverage = 0d;
                    foreach (var target in targets)
                    {
                        boonAverage += GetBoon(target, boonName, buff.Time, buff.Time + 1);
                    }
                    boonAverage /= targets.Length;
                    boonEvents.Add((buff.Time, boonAverage));
                    boonStart = boonAverage;
                }
            }

            var boonEnd = 0d;
            foreach (var target in targets)
            {
                boonEnd += GetBoon(target, boonName, startTime, startTime + 1);
            }
            boonEnd /= targets.Length;

            boonEvents.Add((endTime, boonEnd));
            return boonEvents;
        }

        private List<BuffEvent> GetBuffEventsForTarget(long ID, string target)
        {
            SingleActor? Target = _log.PlayerList.FirstOrDefault(x => x.Account == target);
            if (Target == null)
            {
                Target = _log.FightData.GetMainTargets(_log).FirstOrDefault(x => x.Equals(target));
                if (Target == null)
                {
                    return new();
                }
            }
            return _log.CombatData.GetBuffDataByIDByDst(ID, Target.AgentItem).ToList();
        }

        private List<(double, double)> GetBoonDurationEvents(string[] targets, string boonName, string phaseName, string source)
        {
            var startTime = GetPhaseStart(phaseName);
            var endTime = GetPhaseEnd(phaseName);
            var prevTime = startTime;
            var prevDuration = GetAverageBoonDuration(targets, boonName, startTime);
            var boonEvents = new List<(double, double)>
            {
                { (startTime, prevDuration) }
            };
            prevDuration *= 1000;
            foreach (var Boon in _log.StatisticsHelper.PresentBoons.Where(x => x.Name.Equals(boonName, StringComparison.OrdinalIgnoreCase)))
            {
                var _buffEvents = new List<BuffEvent>();
                foreach(var target in targets)
                {
                    _buffEvents.AddRange(GetBuffEventsForTarget(Boon.ID, target).Where(x => x.By.Name == source || source == ""));
                }
                foreach (var buff in _buffEvents)
                {
                    if (buff.Time < startTime && startTime != 0)
                    {
                        continue;
                    }
                    if (buff.Time > endTime)
                    {
                        break;
                    }
                    var buffApplyEvent = buff as BuffApplyEvent;
                    var buffExtensionEvent = buff as BuffExtensionEvent;
                    var buffRemovedEvent = buff as BuffRemoveAllEvent;
                    if (buffApplyEvent == null && buffExtensionEvent == null && buffRemovedEvent == null)
                    {
                        continue;
                    }
                    var leftoverDuration = prevDuration - (buff.Time - prevTime);
                    if (leftoverDuration < 0)
                    {
                        boonEvents.Add((buff.Time + leftoverDuration, 0));
                        boonEvents.Add((buff.Time, 0));
                        prevDuration = 0;
                    }
                    else
                    {
                        boonEvents.Add((buff.Time - 1, leftoverDuration/1000f));
                        prevDuration = leftoverDuration;
                    }
                    prevDuration -= buffRemovedEvent?.RemovedDuration ?? 0L;
                    prevDuration += buffApplyEvent?.AppliedDuration ?? 0L;
                    prevDuration += buffExtensionEvent?.ExtendedDuration ?? 0L;
                    if (prevDuration < 0)
                    {
                        prevDuration = 0;
                    }
                    boonEvents.Add((buff.Time, prevDuration / 1000f));
                    prevTime = buff.Time;
                }
                if(prevTime < endTime)
                {
                    var leftoverDuration = prevDuration - (endTime - prevTime);
                    if (leftoverDuration < 0)
                    {
                        boonEvents.Add((endTime + leftoverDuration, 0));
                        boonEvents.Add((endTime, 0));
                    }
                    else
                    {
                        boonEvents.Add((endTime - 1, leftoverDuration / 1000f));
                    }
                }
            }

            return boonEvents;
        }

        private double GetBoon(string target, string boonName, long start, long end, long time = 0, bool duration = false)
        {
            SingleActor? Target = _log.PlayerList.FirstOrDefault(x => x.Account == target);
            if (Target == null)
            {
                Target = _log.FightData.GetMainTargets(_log).FirstOrDefault(x => x.Equals("target"));
                if (Target == null)
                {
                    return 0;
                }
            }

            var deathEvents = _log.CombatData.GetDeadEvents(Target.AgentItem);
            var de = deathEvents.FirstOrDefault(x => x.Time <= end);
            if (de != null)
            {
                end = de.Time;
            }
            if(end <= start)
            {
                return -1;
            }

            var Buffs = Target.GetBuffs(BuffEnum.Self, _log, start, end);
            var targetBuffs = new List<Buff>();
            foreach (var buff in 
                _log.StatisticsHelper.PresentBoons.Where(x => x.Name.Equals(boonName, StringComparison.OrdinalIgnoreCase)).Concat(
                    _log.StatisticsHelper.PresentConditions.Where(x => x.Name.Equals(boonName, StringComparison.OrdinalIgnoreCase))))
            {
                Buffs.TryGetValue(buff.ID, out var value);
                if (value != null)
                {
                    var uptime = value.Uptime;
                    if (buff.StackType == BuffStackType.Queue || buff.StackType == BuffStackType.Regeneration)
                    {
                        uptime /= 100f;
                    }
                    return uptime;
                }
            }
            return 0;
        }

        public bool HasPlayer(string accountName)
        {
            return _log.PlayerList.Any(x => x.Account == accountName);
        }

        public double GetBoon(int group, string boonName, string phaseName = "", long time = 0, bool duration = false, bool ignoreKite = false, bool ignoreDead = false)
        {
            var groupMembers = _log.PlayerList.Where(x => x.Group == group);
            List<double> boonUptimes = new();
            if(groupMembers.Count() == 0)
            {
                return 0;
            }
            var highestToughness = groupMembers.Max(x => x.Toughness);
            foreach (var player in groupMembers)
            {
                if(ignoreKite && player.Toughness == highestToughness)
                {
                    continue;
                }
                var actualTime = time == 0 ? GetPhaseEnd(phaseName) : time;
                if (ignoreDead && player.IsDead(_log, actualTime))
                {
                    continue;
                }
                var boonResult = GetBoon(player.Account, boonName, phaseName, time, duration);
                if (boonResult == -1) { continue; }
                boonUptimes.Add(GetBoon(player.Account, boonName, phaseName, time, duration));

            }
            if (boonUptimes.Count == 0)
            {
                return 0;
            }
            return boonUptimes.Average();
        }

        public string[] GetMechanicNames(string phaseName = "", long start = 0, long end = 0)
        {
            if (phaseName != "")
            {
                start = GetPhaseStart(phaseName);
                end = GetPhaseEnd(phaseName);
            }
            var result = _log.MechanicData.GetPresentMechanics(_log, start, end);
            return result.Select(x => x.FullName).ToArray();
        }

        public long[] GetShockwaves(int shockwaveType)
        {
            var guid = GetShockwaveGUID(shockwaveType);
            if (_log.CombatData.TryGetEffectEventsByGUID(guid, out IReadOnlyList<EffectEvent>? shockwaves))
            {
                return shockwaves.Select(x => x.Time).ToArray();
            }
            return [];
        }

        private GUID GetShockwaveGUID(int shockwaveType)
        {
            switch (shockwaveType)
            {
                case 0:
                    return EffectGUIDs.HarvestTempleMordremothShockwave1;
                case 1:
                    return EffectGUIDs.HarvestTempleTsunami1;
                case 2:
                    return EffectGUIDs.HarvestTempleVoidObliteratorShockwave;
                default:
                    throw new NotImplementedException();
            }
        }

        public Mechanic? GetMechanic(string mechanicName, long start, long end)
        {
            return _log.MechanicData.GetPresentMechanics(_log, start, end).FirstOrDefault(x => x.FullName.Equals(mechanicName));
        }

        public (string, long)[] GetMechanicLogsForPlayer(string accountName, string mechanicName, string phaseName = "", long start = 0, long end = 0)
        {
            return GetMechanicLogs(mechanicName, phaseName, start, end).Where(x => x.Item1 == accountName).ToArray();
        }

        public (string, long)[] GetMechanicLogs(string mechanicName, string phaseName = "", long start = 0, long end = 0)
        {
            if (phaseName != "")
            {
                start = GetPhaseStart(phaseName);
                end = GetPhaseEnd(phaseName);
            }
            var mech = GetMechanic(mechanicName, start, end);
            if (mech == null)
            {
                return [];
            }
            var result = _log.MechanicData.GetMechanicLogs(_log, mech, start, end).ToList();
            return result.Select(x => (x.Actor.Account, x.Time - start)).ToArray();
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
                case BuffStackType.StackingUniquePerSrc:
                    return BuffStackTyping.StackingTargetUniqueSrc;
                case BuffStackType.Regeneration:
                    return BuffStackTyping.Regeneration;
                case BuffStackType.Force:
                    return BuffStackTyping.Force;
                default:
                    return BuffStackTyping.Unknown;
            }
        }

        public long GetStealthTiming(string phase)
        {
            var stealthPhase = _log.FightData.GetPhases(_log).Where(x => x.Name.Equals(phase)).FirstOrDefault();
            var MassInvis = _log.CombatData.GetAnimatedCastData(10245).Where(x => x.EndTime > stealthPhase?.Start && x.EndTime < stealthPhase?.End).FirstOrDefault();
            return MassInvis?.EndTime ?? 0L;
        }

        private void CreateFile()
        {
            if (!File.Exists("StealthPhases.txt"))
            {
                File.WriteAllLines("StealthPhases.txt", new string[]
                {
                    "#This file should contain all expected stealth phases in the PhaseName|DescriptiveName format",
                    "Primordus|Primordus",
                    "Kralkatorrik|Kralkatorrik",
                    "Giants|Giants",
                    "Soo-Won 2|Champions"
                });
            }
        }



        public List<(string, string)> GetStealthResult(string accountName, StealthAlgoritmns algoritmn, bool showLate = false)
        {

            var StealthResult = new List<(string, string)>();
            var MassInvis = _log.CombatData.GetAnimatedCastData(10245);
            var buffRemoved = _log.CombatData.GetBuffRemoveAllData(10269);
            var buffInfoEvent = _log.CombatData.GetBuffInfoEvent(10269);
            var revealedRemovedEvent = _log.CombatData.GetBuffRemoveAllData(890);


            foreach (var phase in _expectedStealthPhases)
            {
                var phaseData = _log.FightData.GetPhases(_log).Where(x => x.Name.Equals(phase.Key)).FirstOrDefault();

                if (phaseData == null) continue;
                
                var Invis = MassInvis.Where(x => x.EndTime + 10000 > phaseData.Start && x.EndTime < phaseData.End).FirstOrDefault();
                if (Invis == null)
                {
                    StealthResult.Add((phase.Value, "No MI"));
                    continue;
                }
                switch (algoritmn)
                {
                    case StealthAlgoritmns.OutlierFiltering:
                    case StealthAlgoritmns.MedianTiming:
                        StealthResult.Add((phase.Value, GetStealth(phaseData, accountName, Invis.Time, algoritmn, showLate)));
                        break;
                    case StealthAlgoritmns.Timing:
                        StealthResult.Add((phase.Value, GetStealthTiming(phaseData, accountName, Invis.Time)));
                        break;
                }
            }
            return StealthResult;
        }

        private long GetStealthOutlierFilterTime(long stealthTime)
        {
            List<GW2EIEvtcParser.ParsedData.BuffRemoveAllEvent> RemovedStealthEvents = _log.CombatData.GetBuffRemoveAllData(10269).Where(x => x.Time >= stealthTime && x.Time <= stealthTime + 7000).ToList();
            var trimmedEvents = RemovedStealthEvents.Select(x => x.Time);
            while (true)
            {
                if (trimmedEvents.Count() == 0)
                {
                    return 0;
                }
                long avg = (long)trimmedEvents.Average();
                var first = trimmedEvents.First();
                var last = trimmedEvents.Last();
                var absFirst = Math.Abs(avg - first);
                var absLast = Math.Abs(avg - last);

                if (absFirst > absLast && absFirst > 1000)
                {
                    trimmedEvents = trimmedEvents.Skip(1);
                }
                else if (absLast > absFirst && absLast > 1000)
                {
                    trimmedEvents = trimmedEvents.SkipLast(1);
                }
                else
                {
                    return avg;
                }
            }
        }

        public StealthTimelineCollection GetStealthTimeline()
        {
            var stealthResultsPerPhase = new Dictionary<string, StealthTimeline>();
            var MassInvis = _log.CombatData.GetAnimatedCastData(10245);
            var Phases = _log.FightData.GetPhases(_log);


            foreach (var stealthPhase in _expectedStealthPhases)
            {
                var phase = _log.FightData.GetPhases(_log).FirstOrDefault(y => y.Name == stealthPhase.Key);
                if (phase == null)
                {
                    stealthResultsPerPhase.Add(stealthPhase.Value, new StealthTimeline());
                    continue;
                }
                var killedPhase = Phases.OrderByDescending(x => x.End).FirstOrDefault(x => x.End < phase.Start);
                var invis = MassInvis.FirstOrDefault(x => phase.Start - 10000 < x.EndTime && x.EndTime < phase.End);

                List<StealthResult> stealthResults = new List<StealthResult>();
                long stealthTime = killedPhase.End;
                if (invis == null)
                {
                    foreach (var player in _log.PlayerList)
                    {
                        stealthResults.Add(new StealthResult(player.Account, "No MI"));
                    }
                    continue;
                }
                else
                {
                    foreach (var player in _log.PlayerList)
                    {
                        var stealthEvent = _log.CombatData.GetBuffDataByIDByDst(10269, player.AgentItem).Where(x => x is BuffApplyEvent && x.Time >= invis.Time).FirstOrDefault();
                        if (stealthEvent == null)
                        {
                            stealthResults.Add(new StealthResult(player.Account, "No MI"));
                            continue;
                        }
                        stealthTime = stealthEvent.Time;
                        var _buffEvents = _log.CombatData.GetBuffDataByIDByDst(890, player.AgentItem);
                        var revealed = _buffEvents.FirstOrDefault(x => x.Time >= stealthEvent.Time);
                        var dmgData = _log.CombatData.GetDamageData(player.AgentItem).Where(x => x.Time >= stealthEvent.Time && x.Time <= stealthTime + 6000);

                        bool isKneeling = false;
                        bool isTriggered = false;

                        long endTime = stealthTime + 6000;
                        if (revealed != null)
                        {
                            endTime = revealed.Time;
                        }

                        var naturalConvergences = _log.CombatData.GetAnimatedCastData(31503);
                        var naturalConvergencesByPlayer = naturalConvergences.Where(x => x.Caster.Name.Equals(player.AgentItem.Name));
                        var lingeringNaturalConvergence = naturalConvergencesByPlayer.Where(x => x.EndTime > stealthTime - 8000 && x.Time < endTime).FirstOrDefault();
                        if (lingeringNaturalConvergence != null)
                        {
                            stealthResults.Add(new StealthResult(player.Account, "Lingering Natural Convergence!", stealthTime, stealthTime));
                        }


                        if (player.HasBuff(_log, 42869, stealthTime, endTime - stealthTime) ||
                            player.HasBuff(_log, 62823, stealthTime, endTime - stealthTime))
                        {
                            for (long i = stealthTime; i < endTime; i += 100)
                            {
                                var Buffs = player.GetBuffs(BuffEnum.Self, _log, i, i + 100);
                                var kneel = Buffs.TryGetValue(42869, out var kneelValue);
                                var trigger = Buffs.TryGetValue(62823, out var triggerValue);
                                if (!isKneeling && kneelValue?.Uptime > 0)
                                {
                                    stealthResults.Add(new StealthResult(player.Account, "Kneeling!", i, stealthTime));
                                    isKneeling = true;
                                }
                                if (isKneeling && kneelValue?.Uptime == 0)
                                {
                                    stealthResults.Add(new StealthResult(player.Account, "Stopped Kneeling", i, stealthTime));
                                    isKneeling = false;
                                }
                                if (!isTriggered && triggerValue?.Uptime > 0)
                                {
                                    stealthResults.Add(new StealthResult(player.Account, "Dragon Trigger!", i, stealthTime));
                                    isTriggered = true;
                                }
                                if (isTriggered && triggerValue?.Uptime == 0)
                                {
                                    stealthResults.Add(new StealthResult(player.Account, "Stopped Dragon Trigger", i, stealthTime));
                                    isTriggered = false;
                                }
                            }
                        }

                        if (revealed == null)
                        {
                            stealthResults.Add(new StealthResult(player.Account, "Stealth timeout", invis.EndTime + 6000, invis.EndTime));
                            continue;
                        }

                        var directEventData = dmgData.Where(x => x is DirectHealthDamageEvent);
                        var ascending = directEventData.OrderBy(x => x.Time);
                        var skill = ascending.FirstOrDefault(x => !x.Skill.Name.Equals("Nourishment"));
                        if (skill == null)
                        {
                            stealthResults.Add(new StealthResult(player.Account, "Unknown"));
                        }
                        else
                        {
                            stealthResults.Add(new StealthResult(player.Account, skill.Skill.Name, skill.Time, invis.EndTime));
                        }
                    }
                }
                stealthResults = stealthResults.OrderBy(x => x.Time).ToList();
                stealthResultsPerPhase.Add(stealthPhase.Value, new StealthTimeline(stealthPhase.Value,invis?.Time ?? killedPhase.End, stealthTime, killedPhase.End, invis?.Caster.HasBuff(_log, 1187, invis.Time, invis.EndTime - invis.Time) ?? true, stealthResults));
            }
            return new StealthTimelineCollection(stealthResultsPerPhase);
        }
        private string GetStealth(PhaseData phase, string accountName, long stealthTime, Enums.StealthAlgoritmns stealthAlgoritmn, bool showLate = false)
        {
            if (!_log.PlayerList.Any(x => x.Account == accountName))
            {
                return "";
            }
            List<GW2EIEvtcParser.ParsedData.BuffRemoveAllEvent> RemovedStealthEvents = _log.CombatData.GetBuffRemoveAllData(10269).Where(x => x.Time >= stealthTime && x.Time <= stealthTime + 20000).ToList();
            List<GW2EIEvtcParser.ParsedData.BuffRemoveAllEvent> RemovedRevealedEvents = _log.CombatData.GetBuffRemoveAllData(890).Where(x => x.Time >= stealthTime && x.Time <= stealthTime + 20000).ToList();
            if (RemovedStealthEvents.Count == 0)
            {
                return "";
            }
            if (RemovedRevealedEvents.Where(x => x.To.Name.Contains(accountName)).Count() == 0)
            {
                return ("✓");
            }
            var RemovedEvent = RemovedRevealedEvents.Where(x => x.To.Name.Contains(accountName)).FirstOrDefault();
            if (RemovedEvent == null)
            {
                return ("✓");
            }
            //Check for revealed debuff instead
            var error = "";
            //var player = _log.PlayerAgents.Where(x => x.Name.Contains(accountName)).FirstOrDefault();
            //if (player != null)
            //{
            //    var deaths = _log.CombatData.GetDeadEvents(player);
            //    foreach (var death in deaths)
            //    {
            //        if (death.Time >= stealthTime && death.Time <= stealthTime + 10000)
            //        {
            //            return $"Died";
            //        }
            //    }
            //}
            var RevealedTime = (RemovedEvent.Time + RemovedEvent.RemovedDuration - 3000f);
            var destealthTime = 0L;
            switch (stealthAlgoritmn)
            {
                case StealthAlgoritmns.OutlierFiltering:
                    destealthTime = GetStealthOutlierFilterTime(stealthTime);
                    break;
                case StealthAlgoritmns.MedianTiming:
                    destealthTime = RemovedStealthEvents[(int)Math.Floor(RemovedStealthEvents.Count / 2f)].Time;
                    break;
                case StealthAlgoritmns.Timing:
                    return GetStealthTiming(phase, accountName, stealthTime);
            }
            if (destealthTime == 0)
            {
                return "";
            }
            var delta = destealthTime - RevealedTime;
            if (delta > 1000f || (showLate && Math.Abs(delta) > 1000f))
            {
                var dmgData = _log.CombatData.GetDamageData(RemovedEvent.To).Where(x => x.Time >= stealthTime && x.Time <= RemovedEvent.Time + 10000).OrderBy(x => x.Time);
                var skill = dmgData.FirstOrDefault(x => !x.Skill.Name.Equals("Nourishment") && x is DirectHealthDamageEvent);
                if (skill == null)
                {
                    skill = dmgData.FirstOrDefault(x => !x.Skill.Name.Equals("Nourishment"));
                }
                if (skill == null)
                {
                    if (error == "")
                    {
                        return "Unknown";
                    }
                }
                else
                {
                    var offset = (int)delta / 1000f;
                    bool early = offset > 0;
                    offset *= -1f;
                    if (early)
                    {
                        return $"{offset}s early {skill.Skill.Name}";
                    }
                    else
                    {
                        return $"{offset}s late {skill.Skill.Name}";
                    }
                }
            }
            return "✓";
        }
        private string GetStealthTiming(PhaseData phase, string accountName, long stealthTime)
        {
            var RemovedStealthEvents = _log.CombatData.GetBuffRemoveAllData(10269)
                .Where(x =>
                x.Time >= stealthTime &&
                x.Time <= stealthTime + 3000 &&
                x.To.Name.Contains(accountName)
                ).ToList();

            if (RemovedStealthEvents.Count() > 0)
            {
                var RevealEvent = RemovedStealthEvents.First();
                var dmgData = _log.CombatData.GetDamageData(RevealEvent.To).Where(x => x.Time >= stealthTime).OrderBy(x => x.Time);
                var skill = dmgData.FirstOrDefault(x => !x.Skill.Name.Equals("Nourishment") && x is DirectHealthDamageEvent);
                if (skill == null)
                {
                    skill = dmgData.FirstOrDefault(x => !x.Skill.Name.Equals("Nourishment"));
                }
                if (skill == null)
                {
                    return "Unknown";
                }
                else
                {
                    return $"{-(int)((stealthTime + 3000) - RevealEvent.Time) / 1000f}s {skill.Skill.Name}";
                }
            }
            return "✓";
        }

        private double GetBoonDurationInPhase(string target, string boonName, string phase, long time = 0)
        {
            var phaseData = GetPhaseFromName(phase);
            if (phaseData.Item1 == null)
                return 0;
            return GetBoonDuration(target, boonName, phaseData.Item2 + time);
        }
        private double GetAverageBoonDuration(string[] targets, string boonName, long time)
        {
            var totalDuration = 0d;
            foreach (var target in targets)
            {
                totalDuration += GetBoonDuration(target, boonName, time);
            }
            return totalDuration / targets.Length;
        }

        private double GetBoonDuration(string target, string boonName, long time)
        {
            SingleActor? Target = _log.PlayerList.FirstOrDefault(x => x.Account == target);
            if (Target == null)
            {
                Target = _log.FightData.GetMainTargets(_log).FirstOrDefault(x => x.Equals("target"));
                if (Target == null)
                {
                    return 0;
                }
            }

            var boonStackingType = GetBoonStackType(boonName);
            if (boonStackingType == BuffStackTyping.Unknown)
            {
                return 0;
            }
            if (boonStackingType != BuffStackTyping.Queue && boonStackingType != BuffStackTyping.Regeneration)
            {
                return GetBoon(target, boonName, time, time + 1);
            }

            var maxDuration = 30000L;
            if (boonName == "Swiftness")
            {
                maxDuration = 60000L;
            }

            long currentTime = 0;
            bool firstTime = true;
            long currentDuration = 0;

            foreach (var Boon in _log.StatisticsHelper.PresentBoons.Where(x => x.Name.Equals(boonName, StringComparison.OrdinalIgnoreCase)))
            {
                var _buffEvents = _log.CombatData.GetBuffDataByIDByDst(Boon.ID, Target.AgentItem);
                foreach (var buff in _buffEvents)
                {
                    var buffApplyEvent = buff as BuffApplyEvent;
                    var buffExtensionEvent = buff as BuffExtensionEvent;
                    var buffRemovedEvent = buff as BuffRemoveAllEvent;
                    if (buffApplyEvent == null && buffExtensionEvent == null && buffRemovedEvent == null)
                    {
                        continue;
                    }
                    var duration = 0L;
                    if (buff.Time > time)
                    {
                        break;
                    }
                    if(firstTime)
                    {
                        firstTime = false;
                        currentTime = buff.Time;
                    }
                    duration -= buffRemovedEvent?.RemovedDuration ?? 0;
                    duration += buffApplyEvent?.AppliedDuration ?? 0;
                    duration += buffExtensionEvent?.ExtendedDuration ?? 0;

                    currentDuration -= buff.Time - currentTime; //Figure out delta since last boon application
                    currentDuration = Math.Max(0, currentDuration);//Make sure we don't go negative
                    currentTime = buff.Time;//Update current time
                    currentDuration += duration;//Add boon duration
                    currentDuration = Math.Min(currentDuration, maxDuration);//Make sure we don't go over max duration
                }
            }
            return Math.Ceiling(Math.Max(0, currentDuration - (time - currentTime)) / 1000d);
        }

        private (PhaseData?, long, long) GetPhaseFromName(string phaseName)
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
                if (splitPhaseName.Length == 3)
                {
                    phaseName = splitPhaseName[0].Split(':').First();
                }
                phase = _log.FightData.GetPhases(_log).Where(x => x.Name == phaseName).FirstOrDefault();
                if (phase == null)
                {
                    return (null, 0, 0);
                }
                start = phase.Start;
                duration = phase.End - phase.Start;
                if (splitPhaseName.Length == 3)
                {
                    start = start + long.Parse(splitPhaseName[1]) * 1000;
                    duration = long.Parse(splitPhaseName[2]) * 1000;
                }
            }
            return (phase, start, duration);
        }

        public string[] GetPlayers()
        {
            return _log.PlayerList.Select(x => x.Account).Distinct().ToArray();
        }

        public int[] GetGroups()
        {
            return _log.PlayerList.Select(x => x.Group).Distinct().ToArray();
        }

        public int GetPlayerGroup(string accountName)
        {
            return _log.PlayerList.FirstOrDefault(x => x.Account == accountName)?.Group ?? -1;
        }

        public bool IsPlayerInGroup(string accountName, int group)
        {
            return _log.PlayerList.Any(x => x.Account == accountName && x.Group == group);
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
            if (end == start)
            {
                end = Phase.Item1?.End ?? 0;
            }
            return end;
        }

        private static SettingsFile _shockwaveFile = new SettingsFile("ShockwaveSettings.txt",
            [
                ("MordemothSpeed", $"{1.0131723589421585819568253968254}"),
                ("Soo-WonSpeed", $"{2021.650996/(466332-461905)}"),
                ("VoidObliteratorSpeed", $"{345.905953/(582063-580045)}"),
                ("MordemothDuration", $"{1.6}"),
                ("Soo-WonDuration", $"{4.5}"),
                ("VoidObliteratorDuration", $"{2.5}")
            ]);
        private bool _fileLoaded = false;

        private double _mordemWaveSpeed = 1.0131723589421585819568253968254; //This number is taken from a distance/time calculation based on the on-hit event of the wave
        private double _sooWonSpeed = 2021.650996 / (466332 - 461905);
        private double _obliteratorSpeed = 345.905953 / (582063 - 580045);
        private double _mordemWaveDuration = 1.6;
        private double _sooWonWaveDuration = 4.5;
        private double _obliteratorWaveDuration = 2.5;

        private void LoadShockwaveSettings()
        {
            if(_fileLoaded)
            {
                return;
            }
            _fileLoaded = true;

            double.TryParse(_shockwaveFile.GetSetting("MordemothSpeed"), new CultureInfo("en-US"), out _mordemWaveSpeed);
            double.TryParse(_shockwaveFile.GetSetting("Soo-WonSpeed"), new CultureInfo("en-US"), out _sooWonSpeed);
            double.TryParse(_shockwaveFile.GetSetting("VoidObliteratorSpeed"), new CultureInfo("en-US"), out _obliteratorSpeed);
            double.TryParse(_shockwaveFile.GetSetting("MordemothDuration"), new CultureInfo("en-US"), out _mordemWaveDuration);
            double.TryParse(_shockwaveFile.GetSetting("Soo-WonDuration"), new CultureInfo("en-US"), out _sooWonWaveDuration);
            double.TryParse(_shockwaveFile.GetSetting("VoidObliteratorDuration"), new CultureInfo("en-US"), out _obliteratorWaveDuration);
        }

        private double GetShockwaveDuration(ShockwaveType type)
        {
            LoadShockwaveSettings();
            switch (type)
            {
                case ShockwaveType.Mordemoth:
                    return _mordemWaveDuration;
                case ShockwaveType.SooWon:
                    return _sooWonWaveDuration;
                case ShockwaveType.VoidObliterator:
                    return _obliteratorWaveDuration;
                default:
                    return 0.0;
            }
        }
        private double GetShockwaveSpeed(ShockwaveType type)
        {
            LoadShockwaveSettings();
            switch(type)
            {
                case ShockwaveType.Mordemoth:
                    return _mordemWaveSpeed;
                case ShockwaveType.SooWon:
                    return _sooWonSpeed;
                case ShockwaveType.VoidObliterator:
                    return _obliteratorSpeed;
                default:
                    return 0.0;
            }
        }

        public long GetShockwaveIntersectionTime(string player, ShockwaveType type, Vector3 shockwavePoint, long shockwaveTime)
        {
            var currentTime = shockwaveTime;
            var waveEnd = shockwaveTime + GetShockwaveDuration(type)*1000;
            while (currentTime < waveEnd)
            {
                var Player = _log.PlayerList.FirstOrDefault(x => x.Account == player);
                if(Player?.TryGetCurrentPosition(_log, currentTime, out var Position) ?? false)
                {
                    var playerDistanceToShockwaveOrigin = Vector3.Distance(Position, shockwavePoint);
                    var shockwaveDistance = Convert.ToInt64((currentTime - shockwaveTime) * GetShockwaveSpeed(type));
                    if (shockwaveDistance > playerDistanceToShockwaveOrigin)
                    {
                        return currentTime;
                    }
                }
                currentTime += 1;
            }
            return 0;//Player never intersected the shockwave
        }


        public bool HasStabDuringShockwave(string player, ShockwaveType type, long shockwaveTime, out long intersectionTime)
        {
            var guid = GetShockwaveGUID((int)type);
            if (_log.CombatData.TryGetEffectEventsByGUID(guid, out IReadOnlyList<EffectEvent> shockwaves))
            {
                var shockwave = shockwaves.FirstOrDefault(x => x.Time == shockwaveTime);
                if(shockwave == null)
                {
                    intersectionTime = 0;
                    return true;
                }
                intersectionTime = GetShockwaveIntersectionTime(player, type, shockwave.Position, shockwaveTime);
                if(intersectionTime == 0)
                {
                    return true;//Player never intersected the shockwave, so they were safe either way
                }
                return HasBoonDuringTime(player, "Stability", intersectionTime - 150, intersectionTime + 150);
            }
            intersectionTime = 0;
            return true;//Error retrieving shockwave
        }

        public bool HasBoonDuringTime(string target, string boonName, long start, long end)
        {
            SingleActor? Target = _log.PlayerList.FirstOrDefault(x => x.Account == target);


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
            SingleActor? Target = _log.PlayerList.FirstOrDefault(x => x.Account == player);
            var IsDead = Target?.IsDead(_log, time) ?? false;
            var IsDC = Target?.IsDC(_log, time) ?? false;
            return !IsDead && !IsDC;
        }

        public List<(string, long)> GetDownReasons(string accountName)
        {
            var player = _log.PlayerAgents.FirstOrDefault(x => x.Name.Contains(accountName));
            if(player == null)
            {
                return new();
            }
            var downEvents = _log.CombatData.GetDownEvents(player);
            var deathEvents = _log.CombatData.GetDeadEvents(player);
            var dmgTakenEvents = _log.CombatData.GetDamageTakenData(player);
            List<(string,long)> downedReasons = new();
            foreach(var downed in downEvents)
            {
                var events = dmgTakenEvents.Where(x => Math.Abs(x.Time - downed.Time) < 10).ToList();
                var skillNames = events.Select(x => x.Skill.ID);

                var skill = _log.SkillData.Get(events.First().Skill.ID);
                var name = skill.Name;
                if(int.TryParse(name, out var i))
                {
                    var newName = ForceGetMechanicName(i);
                    if (newName != null)
                    {
                        name = newName;
                    }
                }
                downedReasons.Add((name,downed.Time));
            }
            return downedReasons;
        }

        public string GetSpec(string accountName)
        {
            var Spec = _log.PlayerList.FirstOrDefault(x => x.Account == accountName)?.Spec;
            return Spec?.ToString() ?? "";
        }

        private string? ForceGetMechanicName(long ID)
        {
            switch (ID)
            {
                case 65017:
                    return "Branding Beam";
                default:
                    return null;
            }

        }

        public string[] GetConsumables(string accountName)
        {
            return GetFood(accountName).Concat(GetEnhancements(accountName)).ToArray();
        }

        public string[] GetFood(string accountName)
        {
            var foods = _log.StatisticsHelper.PresentNourishements;
            var foodsByPlayer = foods.Where(x => _log.PlayerList.FirstOrDefault(x => x.Account == accountName)?.HasBuff(_log, x.ID, 0, _log.FightData.FightDuration) ?? false);
            return foodsByPlayer.Select(x => x.Name).ToArray();
        }

        public string[] GetEnhancements(string account)
        {
            var enhancements = _log.StatisticsHelper.PresentEnhancements;
            var enhancementsByPlayer = enhancements.Where(x => _log.PlayerList.FirstOrDefault(x => x.Account == account)?.HasBuff(_log, x.ID, 0, _log.FightData.FightDuration) ?? false);
            return enhancementsByPlayer.Select(x => x.Name).ToArray();
        }

        public bool HasReinforcedArmor(string accountName)
        {
            var player = _log.PlayerList.FirstOrDefault(x => x.Account == accountName);
            if (player == null)
            {
                return false;
            }
            var hasBuff = player.HasBuff(_log, 9283, player.FirstAware, player.LastAware - player.FirstAware); ;
            return hasBuff;
        }


        private List<LastLaugh> GetLastLaughs(string accountName, string phaseName, long[] skillIds)
        {
            var player = _log.PlayerList.FirstOrDefault(x => x.Account == accountName);
            if (player == null)
            {
                return [];
            }
            var damageEvents = _log.CombatData.GetDamageTakenData(player.AgentItem);
            var lastLaughEvents = damageEvents.Where(x => skillIds.Contains(x.SkillID)).ToList();

            List<LastLaugh> lastLaughs = new();
            foreach (var lastLaugh in lastLaughEvents.DistinctBy(x => x.Time))
            {
                if(lastLaugh.HealthDamage+lastLaugh.ShieldDamage == 0)
                {
                    continue;
                }
                lastLaughs.Add(new LastLaugh(lastLaugh.SkillID, lastLaugh.From.Name, accountName, lastLaugh.Time, lastLaugh.HealthDamage, lastLaugh.ShieldDamage));
            }
            return lastLaughs;
        }   

        public List<LastLaugh> GetLastLaughs(string accountName, string phaseName)
        {
            return GetLastLaughs(accountName, phaseName, [64557, 65595, 64585]);
        }

        public List<LastLaugh> GetChampionLastLaugh(string accountName, string phaseName)
        {
            return GetLastLaughs(accountName, phaseName, [64585]);
        }

        public long[] GetZhaitanFearTimings()
        {
            var start = GetPhaseStart("Full Fight");
            var end = GetPhaseEnd("Full Fight");
            var mech = GetMechanic("Zhaitan Scream", start, end);
            if (mech == null)
            {
                return [];
            }
            var result = _log.MechanicData.GetMechanicLogs(_log, mech, start, end).ToList();
            return result.Select(x => x.Time).Distinct().ToArray();
        }

        public (string,long) GetCleanseReactionTime(string player, long fearTime)
        {
            var buffRemovedEvents = _log.CombatData.GetBuffRemoveAllData(791).Where(x => x.To.Name.Contains(player));
            var firstEventAfterFear = buffRemovedEvents.FirstOrDefault(x => x.Time > fearTime-50 && x.Time < fearTime + 6000);
            var name = firstEventAfterFear?.By.GetFinalMaster().Name.Split(':').Last().Split('\0').First();
            return (name ?? "", firstEventAfterFear?.Time - fearTime ?? 0);
        }

        public string[] GetDamageReductionsAtTime(string player, long fearTime)
        {
            var playerAgent = _log.PlayerList.FirstOrDefault(x => x.Account == player);
            var dmgReducts = playerAgent?.GetPresentIncomingDamageModifier(_log);
            var buffs = playerAgent?.GetBuffs(BuffEnum.Self, _log, fearTime, fearTime + 1);
            List<string> activeBuffs = new();

            var presentBuffs = _log.StatisticsHelper.PresentBoons.Where(x => x.Name != "Resistance" && x.Name != "Resolution" && (dmgReducts?.Any(y => y == x.ID) ?? false))
                .Concat(_log.StatisticsHelper.PresentSupbuffs.Where(x => dmgReducts?.Any(y => y == x.ID) ?? false))
                .Concat(_log.StatisticsHelper.PresentOffbuffs.Where(x => dmgReducts?.Any(y => y == x.ID) ?? false))
                .Concat(_log.StatisticsHelper.PresentNourishements.Where(x => dmgReducts?.Any(y => y == x.ID) ?? false))
                .Concat(_log.StatisticsHelper.PresentEnhancements.Where(x => dmgReducts?.Any(y => y == x.ID) ?? false))
                .Concat(_log.StatisticsHelper.PresentGearbuffs.Where(x => dmgReducts?.Any(y => y == x.ID) ?? false))
                .Concat(_log.StatisticsHelper.PresentDefbuffs.Where(x => dmgReducts?.Any(y => y == x.ID) ?? false)).ToList();


            foreach (var buff in presentBuffs)
            {
                if(buffs?.TryGetValue(buff.ID, out var value) ?? false)
                {
                    activeBuffs.Add(buff.Name);
                }
            }

            var barrier = playerAgent.GetCurrentBarrierPercent(_log, fearTime);
            if (barrier > 0)
            {
                activeBuffs.Add($"Barrier {barrier}%");
            }

            return activeBuffs.ToArray();
        }


        public long GetBoonStripDuringPhase(string player, string phase)
        {
            var playerAgent = _log.PlayerList.FirstOrDefault(x => x.Account == player);
            var start = GetPhaseStart(phase);
            var end = GetPhaseEnd(phase);
            var support = playerAgent?.GetToAllySupportStats(_log, start, end);
            return support?.BoonStripCount ?? 0;
        }
    }
}
