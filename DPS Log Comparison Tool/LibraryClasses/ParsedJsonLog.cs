using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool.Enums;
using Bulk_Log_Comparison_Tool.LibraryClasses;
using Bulk_Log_Comparison_Tool.Util;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIJSON.JsonRotation;

public class ParsedJsonLog : IParsedEvtcLog
{
    private readonly JsonLog _log;
    private readonly string _fileName;
    private Dictionary<string, (long, long)> _customPhases = new();
    private Dictionary<string, string>? _expectedStealthPhases = new();

    private readonly List<string> BOONNAMES = [ "Might", "Fury", "Quickness", "Alacrity", "Protection", "Regeneration", "Vigor", "Stability", "Swiftness", "Resistance", "Resolution", "Aegis" ];

    public ParsedJsonLog(JsonLog log, string fileName)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));
        _fileName = fileName.Split('=').Last();
        LoadFile();
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

    public string GetFileName() => _fileName;

    public string GetLogStart() => _log.TimeStart ?? string.Empty;

    public double GetPlayerDps(string accountName, string phaseName = "", bool allTarget = false, bool cumulative = false, bool defiance = false, DamageTyping damageType = DamageTyping.All)
    {
        var playerAccount = _log.Players?.FirstOrDefault(p => string.Equals(p.Account, accountName, StringComparison.OrdinalIgnoreCase));
        if (playerAccount == null) return 0;
        var phaseIndex = GetIndexFromPhase(phaseName);
        if (phaseIndex < 0) return 0;
        var phase = _log.Phases.First(x => x.Name == phaseName);
        var targets = phase.Targets;
        if (!allTarget)
        {
            targets = [targets.First()];
        }
        var dmg = 0;
        foreach(var targetIndex in targets)
        {
            var damageNumbers = playerAccount.DpsTargets[targetIndex][phaseIndex];
            if (cumulative)
            {
                if (defiance)
                {
                    dmg += (int)damageNumbers.BreakbarDamage;
                }
                else
                {
                    dmg += damageNumbers.Damage;
                }
            }
            else
            {
                dmg += damageNumbers.Dps;
            }
        }
        return dmg;
        //return GetPlayerDps(accountName, GetPhaseEnd(phaseName)-GetPhaseStart(phaseName), phaseName, allTarget, cumulative, defiance, damageType);
    }

    public double GetPlayerDps(string accountName, long time = 0, string phaseName = "", bool allTarget = false, bool cumulative = false, bool defiance = false, DamageTyping damageType = DamageTyping.All)
    {
        
        var playerAccount = _log.Players?.FirstOrDefault(p => string.Equals(p.Account, accountName, StringComparison.OrdinalIgnoreCase));
        if (playerAccount == null) return 0;
        var phaseIndex = GetIndexFromPhase(phaseName);
        if (phaseIndex < 0) return 0;
        var phase = _log.Phases.First(x => x.Name == phaseName);

        var targetIndex = phase.Targets.FirstOrDefault();
        var target = _log.Targets[targetIndex];

        var damageNumbers = playerAccount.TargetDamage1S.Select(x => x[phaseIndex]);
        if (!allTarget)
        {
            damageNumbers = [damageNumbers.ElementAt(targetIndex)];
        }
        var cumulDamage = 0;
        foreach (var damageNumber in damageNumbers)
        {
            if (time / 1000 >= damageNumber.Count)
            {
                cumulDamage += damageNumber.Last();
            }
            else
            {
                cumulDamage += damageNumber[(int)(time / 1000)];
            }
        }
        if (cumulative)
        {
            return cumulDamage;
        }
        return cumulDamage / (time / 1000);

    }
    public double GetPlayerDps(string accountName, long start, long end, JsonNPC[] targets, bool cumulative = false, bool defiance = false, DamageTyping damageType = DamageTyping.All)
    {
        var playerObject = _log.Players?.FirstOrDefault(p => string.Equals(p.Account, accountName, StringComparison.OrdinalIgnoreCase));
        if (playerObject == null) return 0;
        var targetDamage = playerObject.TargetDamage1S;
        var fullFightPhase = _log.Phases?.FirstOrDefault(p => string.Equals(p.Name, "Full Fight", StringComparison.OrdinalIgnoreCase));
        foreach (var target in targets)
        {
            for (int i = 0; i < fullFightPhase.Targets.Count; i++)
            {
                var phaseTarget = fullFightPhase.Targets[i];
                if(phaseTarget == target.Id)
                {

                }
            }
        }
        return 0;
    }

    public string[] GetPhases(string[] filter, bool exclusion = true)
    {
        // Get phase names from _log.Phases
        var logPhases = _log.Phases?.Select(p => p.Name ?? string.Empty) ?? Enumerable.Empty<string>();
        // Format custom phases as "{key}|{start}|{duration}"
        var customPhases = _customPhases.Select(kvp => $"{kvp.Key}|{kvp.Value.Item1}|{kvp.Value.Item2}");

        // Concatenate both
        var allPhases = logPhases.Concat(customPhases);

        if (filter == null || filter.Length == 0) return allPhases.ToArray();
        return exclusion
            ? allPhases.Where(x => filter.All(y => !x.Contains(y))).ToArray()
            : allPhases.Where(x => filter.Any(y => x.Contains(y))).ToArray();
    }

    private int GetIndexFromPhase(string phase)
    {
        var logPhases = (_log.Phases?.Select(p => p.Name ?? string.Empty) ?? Enumerable.Empty<string>()).ToArray();
        for (int i = 0; i < logPhases.Count(); i++)
        {
            if (phase.Equals(logPhases[i]))
            {
                return i;
            }
        }
        return -1;
    }

    public void AddPhase(string name, long start, long duration)
    {
        if (!_customPhases.ContainsKey(name))
        {
            _customPhases.Add(name, (start, duration));
        }
    }

    public string[] GetTargets(string phaseName = "")
    {
        if (_log.Targets == null) return Array.Empty<string>();
        // If phaseName is provided, filter targets by phase if possible
        return _log.Targets.Select(t => t.Name ?? string.Empty).ToArray();
    }

    public double GetBoon(string target, string boonName, string phaseName = "", long time = 0, bool duration = false)
    {
        if (duration)
        {
            return -1;
        }
        if (phaseName == "")
        {
            phaseName = GetPhases([]).FirstOrDefault() ?? "";
        }

        var boonID = GetBuffIdFromBoonName(boonName);
        if (boonID == 0) return 0;
        if (_log.BuffMap == null) return 0;
        var player = _log.Players?.FirstOrDefault(p => string.Equals(p.Account, target, StringComparison.OrdinalIgnoreCase));
        if(player == null) return 0;
        var buff = player.BuffUptimes?.FirstOrDefault(b => b.Id == boonID);
        if (buff == null) return 0;
        var buffData = buff.BuffData;

        var phaseStart = GetPhaseStart(phaseName);
        var phaseEnd = time == 0 ? GetPhaseEnd(phaseName) : phaseStart+time;

        var boonTimedEvents = GetBoonTimedEvents(target, boonName, phaseStart,phaseEnd);

        double totalValue = 0;
        double totalTime = boonTimedEvents.Last().Item1 - boonTimedEvents.First().Item1;
        double prevValue = 0;
        double prevTime = 0;

        foreach (var boonTimedEvent in boonTimedEvents)
        {
            if(prevTime == 0)
            {
                prevTime = boonTimedEvent.Item1;
                prevValue = boonTimedEvent.Item2;
                continue;
            }
            totalValue += prevValue * (boonTimedEvent.Item1 - prevTime);
            prevTime = boonTimedEvent.Item1;
            prevValue = boonTimedEvent.Item2;
        }

        return totalValue / totalTime;
    }

    public double GetBoonAtTime(string target, string boonName, long time)
    {
        var boonID = GetBuffIdFromBoonName(boonName);
        if (boonID == 0) return 0;
        return GetBoonAtTime(target, boonID, time);
    }
    public double GetBoonAtTime(string target, long boonID, long time)
    {
        if (_log.BuffMap == null) return 0;
        var player = _log.Players?.FirstOrDefault(p => string.Equals(p.Account, target, StringComparison.OrdinalIgnoreCase));
        if (player == null) return 0;
        var buff = player.BuffUptimes?.FirstOrDefault(b => b.Id == boonID);
        if (buff == null) return 0;
        var buffState = buff.States.Last(x => x[0] <= time)?[1] ?? 0;
        return buffState;
    }

    public List<(double, double)> GetBoonTimedEvents(string target, string boonName, string phaseName = "", string source = "")
    {
        var phaseStart = GetPhaseStart(phaseName);
        var phaseEnd = GetPhaseEnd(phaseName);

        return GetBoonTimedEvents(target, boonName, phaseStart, phaseEnd, source);
    }
    public List<(double, double)> GetBoonTimedEvents(string target, long boonID, string phaseName = "", string source = "")
    {
        var phaseStart = GetPhaseStart(phaseName);
        var phaseEnd = GetPhaseEnd(phaseName);

        return GetBoonTimedEvents(target, boonID, phaseStart, phaseEnd, source);
    }

    private List<(double, double)> GetBoonTimedEvents(string target, string boonName, long start, long end, string source = "")
    {
        var boonID = GetBuffIdFromBoonName(boonName);
        if(boonName == "Quickness")
        {
            Console.Write("bla");
        }
        if (boonID == 0) return [];
        return GetBoonTimedEvents(target, boonID, start, end, source);
    }
    private List<(double, double)> GetBoonTimedEvents(string target, long boonID, long start, long end, string source = "")
    {
        if (_log.BuffMap == null) return [];
        var player = _log.Players?.FirstOrDefault(p => string.Equals(p.Account, target, StringComparison.OrdinalIgnoreCase));
        if (player == null) return [];
        var buff = player.BuffUptimes?.FirstOrDefault(b => b.Id == boonID);
        if (buff == null) return [];

        var boonTimedEvents = new List<(double, double)>();
        boonTimedEvents.Add((start, GetBoonAtTime(target, boonID, start)));

        var prevBoon = 0;
        foreach(var buffState in buff.States)
        {
            if (buffState[0] < start || buffState[0] > end) continue;
            boonTimedEvents.Add((buffState[0], buffState[1]));
        }

        boonTimedEvents.Add((end, GetBoonAtTime(target, boonID, end)));
        return boonTimedEvents;
    }

    public double GetBoon(int group, string boonName, string phaseName = "", long time = 0, bool duration = false, bool ignoreKite = false, bool dead = false)
    {
        //todo: Implement dead
        var playercount = 0;
        double boontotal = 0;
        foreach(var player in _log.Players)
        {
            if (IsPlayerInGroup(player.Account, group))
            {
                playercount++;
                boontotal += GetBoon(player.Account, boonName, phaseName, time, duration);
            }
        }
        if (playercount == 0) return 0;
        return boontotal/playercount;
    }

    private long GetBuffIdFromBoonName(string boonName)
    {
        if (_log.BuffMap == null) return 0;
        foreach(var buff in _log.BuffMap)
        {
            if (string.Equals(buff.Value.Name, boonName, StringComparison.OrdinalIgnoreCase))
            {
                long.TryParse(buff.Key.TrimStart('b'), out var id);
                return id;
            }
        }
        return 0;
    }

    public long[] GetShockwaves(int shockwaveType)
    {
        // Not directly supported in JSON, requires custom logic
        return Array.Empty<long>();
    }

    public bool HasPlayer(string accountName)
    {
        if (_log.Players == null) return false;
        return _log.Players.Any(p => string.Equals(p.Account, accountName, StringComparison.OrdinalIgnoreCase));
    }

    public bool HasStabDuringShockwave(string player, ShockwaveType type, long shockwaveTime, out long intersectionTime)
    {
        intersectionTime = 0;
        // Not directly supported in JSON, requires custom logic
        //throw new NotImplementedException("Shockwave stability check not implemented.");
        return false;
    }

    public bool HasBoonDuringTime(string target, string boonName, long start, long end)
    {
        var timedEvents = GetBoonTimedEvents(target, boonName, start, end);
        return timedEvents.Any(x => x.Item2 != 0 && x.Item1 >= start && x.Item1 <= end);
    }

    public bool IsAlive(string player, long time)
    {
        var playerObject = _log.Players?.FirstOrDefault(p => string.Equals(p.Account, player, StringComparison.OrdinalIgnoreCase));
        if (playerObject == null) return false;
        var dead = playerObject.CombatReplayData?.Dead?.Any(x => x[0] <= time && x[1] >= time) ?? false;
        var dc = playerObject.CombatReplayData?.Dc?.Any(x => x[0] <= time && x[1] >= time) ?? false;
        return !(dead || dc);
    }

    public BuffStackTyping GetBoonStackType(string boonName)
    {
        if (_log.BuffMap == null) return BuffStackTyping.Unknown;
        var buff = _log.BuffMap.Values.FirstOrDefault(b => string.Equals(b.Name, boonName, StringComparison.OrdinalIgnoreCase));
        if (buff == null) return BuffStackTyping.Unknown;
        // Guessing based on stacking property
        return buff.Stacking ? BuffStackTyping.Stacking : BuffStackTyping.Queue;
    }

    public List<(string, string)> GetStealthResult(string accountName, StealthAlgoritmns algoritmn, bool showLate = false)
    {
        var stealthTimeline = GetStealthTimeline();
        var stealthResults = new List<(string, string)>();
        var stealthResult = stealthTimeline.Results;
        if (stealthResult == null) return stealthResults;
        foreach (var stealth in stealthResult)
        {
            var hadStealth = false;
            foreach(var sr in stealth.Value.Results.Where(x => x.Player == accountName))
            {
                hadStealth = true;
                stealthResults.Add((stealth.Key, sr.Reason + $" at {sr.Time/1000.0}"));
            }
            if (!hadStealth)
            {
                stealthResults.Add((stealth.Key, "No Stealth"));
            }
        }
        return stealthResults;
    }

    public StealthTimelineCollection GetStealthTimeline()
    {
        var stealthResultsPerPhase = new Dictionary<string, StealthTimeline>();
        var Phases = _log.Phases;
        var MassInvis = _log.Players.SelectMany(x => x.Rotation.Where(x => x.Id == 10245).SelectMany(x => x.Skills));
        JsonSkill? lastStealth = null;

        foreach(var stealthPhase in _expectedStealthPhases)
        {
            var phase = _log.Phases.FirstOrDefault(y => y.Name == stealthPhase.Key);
            if (phase == null)
            {
                stealthResultsPerPhase.Add(stealthPhase.Value, new StealthTimeline());
                continue;
            }


            var killedPhase = Phases.OrderByDescending(x => x.End).FirstOrDefault(x => x.End < phase.Start);
            var invis = MassInvis.FirstOrDefault(x => phase.Start - 10000 < x.CastTime+x.Duration && x.CastTime < phase.End);
            lastStealth = invis;

            List<StealthResult> stealthResults = new List<StealthResult>();
            long stealthTime = killedPhase.End;
            if (invis == null)
            {
                foreach (var player in _log.Players)
                {
                    stealthResults.Add(new StealthResult(player.Account, "No MI"));
                }
                continue;
            }
            else
            {
                foreach (var player in _log.Players)
                {
                    var stealths = GetBoonTimedEvents(player.Account, "Hide in Shadows", "Full Fight");
                    var stealthEvent = stealths.FirstOrDefault(x => x.Item2 == 1 && x.Item1 > invis.CastTime && x.Item1 < invis.CastTime + invis.Duration + 6000);
                    if (stealthEvent == default)
                    {
                        stealthResults.Add(new StealthResult(player.Account, "No MI"));
                        continue;
                    }
                    stealthTime = (long)stealthEvent.Item1;
                    var revealed = stealths.FirstOrDefault(x => x.Item2 == 0 && x.Item1 > stealthEvent.Item1);


                    bool isKneeling = false;
                    bool isTriggered = false;

                    var naturalConvergences = player.Rotation.FirstOrDefault(x => x.Id == 31503);
                    var lingeringNaturalConvergence = naturalConvergences?.Skills?.Where(x => x.CastTime+x.Duration > stealthTime - 8000 && x.CastTime < revealed.Item1).FirstOrDefault();
                    if (lingeringNaturalConvergence != null)
                    {
                        stealthResults.Add(new StealthResult(player.Account, "Lingering Natural Convergence!", stealthTime, stealthTime));
                    }

                    var kneel = GetBoonTimedEvents(player.Account, 42869, "Full Fight");
                    var dragonTrigger = GetBoonTimedEvents(player.Account, 62823, "Full Fight");

                    foreach(var kneelTiming in kneel)
                    {
                        if (kneelTiming.Item1 > stealthTime && kneelTiming.Item1 < revealed.Item1)
                        {
                            var text = kneelTiming.Item2 > 0 ? "Kneeling!" : "Stopped Kneeling";
                            stealthResults.Add(new StealthResult(player.Account, text, (long)kneelTiming.Item1, stealthTime));
                        }
                    }
                    foreach(var dt in dragonTrigger)
                    {
                        if (dt.Item1 > stealthTime && dt.Item1 < revealed.Item1)
                        {
                            var text = dt.Item2 > 0 ? "Dragon Trigger!" : "Stopped Dragon Trigger";
                            stealthResults.Add(new StealthResult(player.Account, text, (long)dt.Item1, stealthTime));
                        }
                    }

                    if (revealed == default)
                    {
                        stealthResults.Add(new StealthResult(player.Account, "Stealth timeout", stealthTime + 6000, stealthTime));
                        continue;
                    }

                    stealthResults.Add(new StealthResult(player.Account, "Revealed", (long)revealed.Item1, stealthTime));

                    //var targets = _log.Phases.FirstOrDefault(x => x.Name == stealthPhase.Value)?.Targets;

                    ////stealthResults.Add(new StealthResult(player.Account, "revealed", (long)revealed.Item1, stealthTime));

                    //var revealingSkills = player.Rotation
                    //    .Where(rotation => rotation.Skills.Any(skill => skill.CastTime + skill.Duration <= revealed.Item1 && skill.TimeGained >= 0))
                    //    .OrderByDescending(rotation => rotation.Skills
                    //    .Where(skill => skill.CastTime + skill.Duration <= revealed.Item1 && skill.TimeGained >= 0)
                    //    .Max(skill => skill.CastTime + skill.Duration));

                    //var revealingSkillName = "";
                    //JsonRotation? revealingSkill = null;
                    //foreach (var rs in revealingSkills)
                    //{
                    //    if (!(_log.Targets.Any(x => x.TotalDamageTaken.Any(x => x.Any(x => x.Id == rs?.Id && x.TotalDamage > 0))))) continue;
                    //    var rsName = _log.SkillMap["s" + rs?.Id].Name;
                    //    if (rsName == null || rsName.Contains("Sigil") || rsName.Contains("Nourishment") || rsName.Contains("Weapon Swap"))
                    //    {
                    //        continue; // Skip sigils and nourishment
                    //    }
                    //    revealingSkill = rs;
                    //    revealingSkillName = rsName;
                    //    break;
                    //}

                    //if (revealingSkillName == null)
                    //{
                    //    stealthResults.Add(new StealthResult(player.Account, "Lingering skill"));
                    //}
                    //else
                    //{
                    //    var skill = revealingSkill?.Skills
                    //        .Where(s => s.CastTime + s.Duration <= revealed.Item1 && s.TimeGained >= 0)
                    //        .OrderByDescending(s => s.CastTime + s.Duration)
                    //        .FirstOrDefault();
                    //    stealthResults.Add(new StealthResult(player.Account, revealingSkillName, skill?.CastTime ?? 0, stealthTime));
                    //}
                }
            }
            stealthResults = stealthResults.OrderBy(x => x.Time).ToList();
            stealthResultsPerPhase.Add(stealthPhase.Value, new StealthTimeline(stealthPhase.Value, invis?.CastTime ?? killedPhase.End, stealthTime, killedPhase.End, lastStealth?.Quickness > 0.9, stealthResults));
        }
        return new StealthTimelineCollection(stealthResultsPerPhase);
    }


    public long GetStealthTiming(string phase)
    {
        var stealthPhase = _log.Phases.Where(x => x.Name.Equals(phase)).FirstOrDefault();
        var rotations = _log.Players.SelectMany(x => x.Rotation.Where(x => x.Id == 10245).SelectMany(x => x.Skills));
        var MassInvis = rotations.Where(x => x.CastTime > stealthPhase?.Start - 10000 && x.CastTime < stealthPhase?.End).FirstOrDefault();
        var time = MassInvis?.CastTime + MassInvis?.Duration;
        return time ?? 0L;
    }



    private List<(long Time, string MechanicName)> GetAllMechanicsForPlayer(string accountName)
    {
        var playerObject = _log.Players?.FirstOrDefault(p => string.Equals(p.Account, accountName, StringComparison.OrdinalIgnoreCase));
        var charName = playerObject?.Name ?? string.Empty;
        var result = new List<(long, string)>();
        if (_log.Mechanics == null) return result;

        foreach (var mechanic in _log.Mechanics)
        {
            if (mechanic.MechanicsData == null) continue;
            foreach (var entry in mechanic.MechanicsData)
            {
                if (string.Equals(entry.Actor, charName, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add((entry.Time, mechanic.FullName ?? string.Empty));
                }
            }
        }
        return result.OrderBy(x => x.Item1).ToList();
    }
    public List<(string, long)> GetDownReasons(string accountName)
    {
        var playerObject = _log.Players?.FirstOrDefault(p => string.Equals(p.Account, accountName, StringComparison.OrdinalIgnoreCase));
        if (playerObject == null) return new List<(string, long)>();
        var downReasons = new List<(string, long)>();
        var combatReplayDown = playerObject.CombatReplayData?.Down;
        var playerMechanics = GetAllMechanicsForPlayer(accountName);
        foreach (var down in combatReplayDown)
        {
            var downTiming = down.First();
            var downMechanic = playerMechanics.Last(x => x.Item1 <= downTiming);
            downReasons.Add((downMechanic.Item2, downTiming));
        }
        return downReasons;
    }

    public string[] GetPlayers()
    {
        if (_log.Players == null) return Array.Empty<string>();
        return _log.Players.Select(p => p.Account ?? string.Empty).ToArray();
    }

    public string GetSpec(string accountName)
    {
        if (_log.Players == null) return string.Empty;
        var player = _log.Players.FirstOrDefault(p => string.Equals(p.Account, accountName, StringComparison.OrdinalIgnoreCase));
        return player?.Profession ?? string.Empty;
    }

    public int[] GetGroups()
    {
        if (_log.Players == null) return Array.Empty<int>();
        return _log.Players.Select(p => p.Group).Distinct().ToArray();
    }

    public int GetPlayerGroup(string accountName)
    {
        if (_log.Players == null) return -1;
        return _log.Players.FirstOrDefault(p => string.Equals(p.Account, accountName, StringComparison.OrdinalIgnoreCase))?.Group ?? -1;
    }

    public bool IsPlayerInGroup(string accountName, int group)
    {
        if (_log.Players == null) return false;
        return _log.Players.Any(p => string.Equals(p.Account, accountName, StringComparison.OrdinalIgnoreCase) && p.Group == group);
    }

    public IEnumerable<string> GetBoonNames()
    {
        if (_log.BuffMap == null) return Enumerable.Empty<string>();
        return _log.BuffMap.Values.Where(x => BOONNAMES.Contains(x.Name)).Select(b => b.Name ?? string.Empty);
    }

    public string[] GetMechanicNames(string phaseName = "", long start = 0, long end = 0)
    {
        if (_log.Mechanics == null) return Array.Empty<string>();
        return _log.Mechanics.Select(m => m.FullName ?? string.Empty).ToArray();
    }

    public (string, long)[] GetMechanicLogsForPlayer(string accountName, string mechanicName, string phaseName = "", long start = 0, long end = 0)
    {
        var playerObject = _log.Players?.FirstOrDefault(p => string.Equals(p.Account, accountName, StringComparison.OrdinalIgnoreCase));

        if (playerObject == null) return Array.Empty<(string, long)>();

        return GetMechanicLogs(mechanicName, phaseName, start, end)
            .Where(x => string.Equals(x.Item1, playerObject.Account, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    public (string, long)[] GetMechanicLogs(string mechanicName, string phaseName = "", long start = 0, long end = 0)
    {
        var mechanics = _log.Mechanics;
        if (mechanics == null) return [];
        var mechs = mechanics.FirstOrDefault(x => x.FullName.Equals(mechanicName));
        if (mechs == null) return [];

        var startTime = start;
        var endTime = end;
        
        if(phaseName != "")
        {
            startTime = GetPhaseStart(phaseName);
            endTime = GetPhaseEnd(phaseName);
        }

        return mechs.MechanicsData
            .Select(x => (GetAccountNameFromCharacterName(x.Actor ?? ""), x.Time))
            .Where(x => x.Time >= startTime && x.Time <= endTime)
            .ToArray();
    }

    private string GetAccountNameFromCharacterName(string characterName)
    {
        var playerObject = _log.Players?.FirstOrDefault(p => string.Equals(p.Name, characterName, StringComparison.OrdinalIgnoreCase));
        return playerObject?.Account ?? "";
    }

    public long GetPhaseStart(string phase)
    {
        if (_log.Phases == null) return 0;
        var p = _log.Phases.FirstOrDefault(ph => string.Equals(ph.Name, phase, StringComparison.OrdinalIgnoreCase));
        return p?.Start ?? 0;
    }

    public long GetPhaseEnd(string phase)
    {
        if (_log.Phases == null) return 0;
        var p = _log.Phases.FirstOrDefault(ph => string.Equals(ph.Name, phase, StringComparison.OrdinalIgnoreCase));
        return p?.End ?? 0;
    }

    public string[] GetStealthPhases()
    {
        return _expectedStealthPhases.Keys.ToArray();
    }

    public string[] GetConsumables(string account)
    {
        var playerObject = _log.Players?.FirstOrDefault(p => string.Equals(p.Account, account, StringComparison.OrdinalIgnoreCase));
        if (playerObject == null) return Array.Empty<string>();
        var consumables = new List<string>();
        foreach(var consumable in playerObject.Consumables)
        {
            _log.BuffMap.TryGetValue("b" + consumable.Id, out var buffdesc);
            consumables.Add(buffdesc.Name);
        }

        return consumables.ToArray();
    }

    public bool HasReinforcedArmor(string accountName)
    {
        return HasBoonDuringTime(accountName, "Reinforced Armor", 0, GetPhaseEnd("Full Fight"));
    }

    public List<LastLaugh> GetLastLaughs(string accountName, string phaseName)
    {
        var playerObject = _log.Players?.FirstOrDefault(p => string.Equals(p.Account, accountName, StringComparison.OrdinalIgnoreCase));
        if (playerObject == null) return new List<LastLaugh>();
        var mechanics = _log.Mechanics;
        if (mechanics == null) return new List<LastLaugh>();
        var lastLaughs = mechanics.First(x => x.Name == "VoidExp.H");
        // Not directly supported in JSON, requires custom logic
        return lastLaughs.MechanicsData
            .Where(x => x.Actor == playerObject.Name)
            .Select(x => new LastLaugh(0,"",accountName,x.Time,0,0)).ToList();
    }

    public List<LastLaugh> GetChampionLastLaugh(string accountName, string phaseName)
    {
        var playerObject = _log.Players?.FirstOrDefault(p => string.Equals(p.Account, accountName, StringComparison.OrdinalIgnoreCase));
        if (playerObject == null) return new List<LastLaugh>();
        var mechanics = _log.Mechanics;
        if (mechanics == null) return new List<LastLaugh>();
        var lastLaughs = mechanics.Where(x => x.Name == "VoidExp.H");
        // Not directly supported in JSON, requires custom logic
        return new List<LastLaugh>();
    }

    public long[] GetZhaitanFearTimings()
    {
        var mechs = GetMechanicLogs("Zhaitan Scream", "Full Fight");
        return mechs.Select(x => x.Item2).Distinct().ToArray();
    }

    public (string, long) GetCleanseReactionTime(string player, long fearTime)
    {
        var playerObject = _log.Players?.FirstOrDefault(p => string.Equals(p.Account, player, StringComparison.OrdinalIgnoreCase));
        if (playerObject == null) return ("", 0);
        var currentTime = fearTime;
        double fear = 1;
        while(fear == 1)
        {
            currentTime += 1;
            fear = GetBoonAtTime(player, "fear", currentTime);
        }
        return ("", currentTime- fearTime);
    }
    /*
        public (string,long) GetCleanseReactionTime(string player, long fearTime)
        {
            var buffRemovedEvents = _log.CombatData.GetBuffRemoveAllData(791).Where(x => x.To.Name.Contains(player));
            var firstEventAfterFear = buffRemovedEvents.FirstOrDefault(x => x.Time > fearTime-50 && x.Time < fearTime + 6000);
            var name = firstEventAfterFear?.By.GetFinalMaster().Name.Split(':').Last().Split('\0').First();
            return (name ?? "", firstEventAfterFear?.Time - fearTime ?? 0);
        }
     */


    public string[] GetDamageReductionsAtTime(string player, long fearTime)
    {
        var playerObject = _log.Players?.FirstOrDefault(p => string.Equals(p.Account, player, StringComparison.OrdinalIgnoreCase));
        if (playerObject == null) return Array.Empty<string>();
        var buff = playerObject.BuffUptimes?.FirstOrDefault(b => b.Id == 0);
        if (buff == null) return Array.Empty<string>();
        var buffState = buff.States.Last(x => x[0] <= fearTime)?[1] ?? 0;
        return [];
    }

    public long GetBoonStripDuringPhase(string player, string phase)
    {
        var playerObject = _log.Players?.FirstOrDefault(p => string.Equals(p.Account, player, StringComparison.OrdinalIgnoreCase));
        var phaseIndex = GetIndexFromPhase(phase);
        var supportPhases = playerObject.Support;
        if(supportPhases == null || phaseIndex < 0 || phaseIndex >= supportPhases.Count) return 0;
        var supportPhase = supportPhases[phaseIndex];
        return supportPhase.BoonStrips;
    }
}
