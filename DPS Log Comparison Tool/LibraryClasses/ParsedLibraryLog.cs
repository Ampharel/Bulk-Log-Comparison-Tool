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

        public double GetPlayerDps(string accountName, string phaseName = "", bool allTarget = false, bool cumulative = false, bool defiance = false, DamageTyping damageType = DamageTyping.All)
        {
            var phase = GetPhaseFromName(phaseName);
            if (phase.Item1 == null)
                return 0;

            IReadOnlyList<AbstractSingleActor>? target = null;
            if (phaseName == "Full Fight" && allTarget)
            {
                target = _log.FightData.GetPhases(_log).SelectMany(x => x.Targets).DistinctBy(x => x.Character).ToArray();
            }
            else
            {
                target = phase.Item1.Targets;
            }
            if (target == null)
                return 0;
            return GetPlayerDps(accountName, GetPhaseStart(phaseName), GetPhaseEnd(phaseName), target.ToArray(), cumulative, defiance, damageType);
        }

        public double GetPlayerDps(string accountName, long start, long end, AbstractSingleActor[] targets, bool cumulative = false, bool defiance = false, DamageTyping damageType = DamageTyping.All)
        {
            var player = _log.PlayerList.FirstOrDefault(x => x.Account == accountName);
            if (player == null) return 0;
            if(targets == null)
            {
                return 0;
            }

            double dps = 0;
            foreach (var target in targets)
            {
                if (target == null) return 0;

                var DamageList = player.GetDPSStats(target, _log, start, end);

                if (defiance)
                {
                    dps += DamageList.BreakbarDamage;
                    continue;
                }

                if (!cumulative)
                {
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
                else
                {
                    if ((damageType & DamageTyping.Power) != 0)
                    {
                        dps += DamageList.PowerDamage;
                    }
                    if ((damageType & DamageTyping.Condition) != 0)
                    {
                        dps += DamageList.CondiDamage;
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

        private double GetBoon(string target, string boonName, long start, long end, long time = 0, bool duration = false)
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
                    var uptime = value.Uptime;
                    if(Boon.StackType == BuffStackType.Queue || Boon.StackType == BuffStackType.Regeneration)
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

        public double GetBoon(int group, string boonName, string phaseName = "", long time = 0, bool duration = false)
        {
            var groupMembers = _log.PlayerList.Where(x => x.Group == group);
            List<double> boonUptimes = new();
            foreach (var player in groupMembers)
            {
                boonUptimes.Add(GetBoon(player.Account, boonName, phaseName, time, duration));
            }
            if(boonUptimes.Count == 0)
            {
                return 0;
            }
            return boonUptimes.Average();
        }

        public string[] GetMechanicNames(string phaseName = "", long start = 0, long end = 0)
        {
            if(phaseName != "")
            {
                start = GetPhaseStart(phaseName);
                end = GetPhaseEnd(phaseName);
            }
            var result = _log.MechanicData.GetPresentMechanics(_log,start,end);
            return result.Select(x => x.FullName).ToArray();
        }

        public long[] GetShockwaves(int shockwaveType)
        {
            var guid = GetShockwaveGUID(shockwaveType);
            if (_log.CombatData.TryGetEffectEventsByGUID(guid, out IReadOnlyList<EffectEvent> shockwaves))
            {
                return shockwaves.Select(x => x.Time).ToArray();
            }
            return [];
        }

        private string GetShockwaveGUID(int shockwaveType)
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

        public Mechanic? GetMechanic(string mechanicName,long start, long end)
        {  
            return _log.MechanicData.GetPresentMechanics(_log, start, end).FirstOrDefault(x => x.FullName.Equals(mechanicName));
        }

        public (string,long)[] GetMechanicLogs(string mechanicName, string phaseName = "", long start = 0, long end = 0)
        {
            if (phaseName != "")
            {
                start = GetPhaseStart(phaseName);
                end = GetPhaseEnd(phaseName);
            }
            var mech = GetMechanic(mechanicName, start, end);
            if(mech == null)
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

        public long GetStealthTiming(string phase)
        {
            var stealthPhase = _log.FightData.GetPhases(_log).Where(x => x.Name.Equals(phase)).FirstOrDefault();
            var MassInvis = _log.CombatData.GetAnimatedCastData(10245).Where(x => x.EndTime > stealthPhase?.Start && x.EndTime < stealthPhase?.End).FirstOrDefault();
            return MassInvis?.EndTime ?? 0L;
        }

        public List<(string, string)> GetStealthResult(string accountName, StealthAlgoritmns algoritmn, bool showLate = false)
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
                    if (ph.Name.Contains("Breakbar"))
                    {
                        continue;
                    }
                    FromPhase = ph;
                    if (ph.Start != 0 && ph.End > Invis.EndTime)
                    {
                        break;
                    }
                }
                if (FromPhase == null)
                {
                    continue;
                }

                switch (algoritmn)
                {
                    case StealthAlgoritmns.OutlierFiltering:
                    case StealthAlgoritmns.MedianTiming:
                        StealthResult.Add((FromPhase.Name, GetStealth(FromPhase, accountName, Invis.EndTime, algoritmn, showLate)));
                        break;
                    case StealthAlgoritmns.Timing:
                        StealthResult.Add((FromPhase.Name, GetStealthTiming(FromPhase, accountName, Invis.EndTime)));
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
                if(trimmedEvents.Count() == 0)
                {
                    return 0;
                }
                long avg = (long)trimmedEvents.Average();
                var first = trimmedEvents.First();
                var last = trimmedEvents.Last();
                var absFirst = Math.Abs(avg - first);
                var absLast = Math.Abs(avg - last);

                if(absFirst > absLast && absFirst > 1000)
                {
                    trimmedEvents = trimmedEvents.Skip(1);
                }
                else if(absLast > absFirst && absLast > 1000)
                {
                    trimmedEvents = trimmedEvents.SkipLast(1);
                }
                else
                {
                    return avg;
                }
            }
        }

        private string GetStealth(PhaseData phase, string accountName, long stealthTime, Enums.StealthAlgoritmns stealthAlgoritmn, bool showLate = false)
        {
            List<GW2EIEvtcParser.ParsedData.BuffRemoveAllEvent> RemovedStealthEvents = _log.CombatData.GetBuffRemoveAllData(10269).Where(x => x.Time >= stealthTime && x.Time <= stealthTime + 20000).ToList();
            List<GW2EIEvtcParser.ParsedData.BuffRemoveAllEvent> RemovedRevealedEvents = _log.CombatData.GetBuffRemoveAllData(890).Where(x => x.Time >= stealthTime && x.Time <= stealthTime + 20000).ToList();
            if (RemovedStealthEvents.Count == 0)
            {
                return "";
            }
            if (RemovedRevealedEvents.Where(x => x.To.Name.Contains(accountName)).Count() == 0)
            {
                return("✓");
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
            if(destealthTime == 0)
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

        private long GetBoonDurationInPhase(string target, string boonName, string phase, long time = 0)
        {
            var phaseData = GetPhaseFromName(phase);
            if (phaseData.Item1 == null)
                return 0;
            return GetBoonDuration(target, boonName, phaseData.Item2 + time * 1000);
        }
        private long GetBoonDuration(string target, string boonName, long time)
        {
            AbstractSingleActor? Target = _log.PlayerList.FirstOrDefault(x => x.Account == target);
            if (Target == null)
            {
                Target = _log.FightData.GetMainTargets(_log).FirstOrDefault(x => x.Equals("target"));
                if (Target == null)
                {
                    return 0;
                }
            }

            var maxDuration = 30000L;
            if(boonName == "Swiftness")
            {
                maxDuration = 60000L;
            }

            long currentTime = 0;
            long currentDuration = 0;

            foreach (var Boon in _log.StatisticsHelper.PresentBoons.Where(x => x.Name.Equals(boonName, StringComparison.OrdinalIgnoreCase)))
            {
                var _buffEvents = _log.CombatData.GetBuffDataByIDByDst(Boon.ID, Target.AgentItem);
                foreach (var buff in _buffEvents)
                {
                    var buffApplyEvent = buff as BuffApplyEvent;
                    if(buffApplyEvent == null)
                    {
                        continue;
                    }
                    if (buffApplyEvent.Time > time)
                    {
                        break;
                    }
                    currentDuration -= buffApplyEvent.Time - currentTime;
                    currentDuration = Math.Max(0, currentDuration);
                    currentDuration = Math.Min(currentDuration, maxDuration);
                    currentTime = buffApplyEvent.Time;
                    currentDuration += buffApplyEvent.AppliedDuration;
                }
            }
            return Math.Max(0, currentDuration - (time - currentTime)) / 1000L;
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

        private const double _mordemWaveSpeed = 1209.552445 / (302100 - 301134); //These numbers are analyzed timestamps and positions from the log https://dps.report/gx23-20241004-155640_void
        private const double _sooWonSpeed = 2021.650996 / (466332 - 461905);
        private const double _obliteratorSpeed = 345.905953 / (582063 - 580045);
        private const double _mordemWaveDuration = 1.6;
        private const double _sooWonWaveDuration = 4.5;
        private const double _obliteratorWaveDuration = 2.5;

        private double GetShockwaveDuration(ShockwaveType type)
        {
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

        public long GetShockwaveIntersectionTime(string player, ShockwaveType type, Point3D shockwavePoint, long shockwaveTime)
        {
            var currentTime = shockwaveTime;
            var waveEnd = shockwaveTime + GetShockwaveDuration(type)*1000;
            while (currentTime < waveEnd)
            {
                var Player = _log.PlayerList.FirstOrDefault(x => x.Account == player);
                var playerPosition = Player?.GetCurrentPosition(_log, currentTime) ?? new Point3D(0, 0);
                var playerDistanceToShockwaveOrigin = Vector3.Distance(playerPosition.ToVector3(), shockwavePoint.ToVector3());
                var shockwaveDistance = Convert.ToInt64((currentTime - shockwaveTime) * GetShockwaveSpeed(type));
                if (shockwaveDistance > playerDistanceToShockwaveOrigin)
                {
                    return currentTime;
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
                return HasBoonDuringTime(player, "Stability", intersectionTime - 100, intersectionTime + 100);
            }
            intersectionTime = 0;
            return true;//Error retrieving shockwave
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
            var IsDead = Target?.IsDead(_log, time) ?? false;
            var IsDC = Target?.IsDC(_log, time) ?? false;
            return !IsDead && !IsDC;
        }

        public List<string> GetDownReasons(string accountName)
        {
            var downEvents = _log.CombatData.GetDownEvents(_log.PlayerAgents.FirstOrDefault(x => x.Name.Contains(accountName)));
            var deathEvents = _log.CombatData.GetDeadEvents(_log.PlayerAgents.FirstOrDefault(x => x.Name.Contains(accountName)));
            var dmgTakenEvents = _log.CombatData.GetDamageTakenData(_log.PlayerAgents.FirstOrDefault(x => x.Name.Contains(accountName)));
            List<string> downedReasons = new();
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
                downedReasons.Add(name);
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
    }
}
