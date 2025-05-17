using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool.Enums;
using Bulk_Log_Comparison_Tool.LibraryClasses;
using Bulk_Log_Comparison_Tool.Util;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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
        _fileName = fileName;
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
        var targetIndex = phase.Targets.First();
        var damageNumbers = playerAccount.DpsTargets[targetIndex][phaseIndex];
        if(cumulative)
        {
            return damageNumbers.Damage;
        }
        return damageNumbers.Dps;
        //return GetPlayerDps(accountName, GetPhaseEnd(phaseName)-GetPhaseStart(phaseName), phaseName, allTarget, cumulative, defiance, damageType);
    }

    public double GetPlayerDps(string accountName, long time = 0, string phaseName = "", bool allTarget = false, bool cumulative = false, bool defiance = false, DamageTyping damageType = DamageTyping.All)
    {
        
        var playerAccount = _log.Players?.FirstOrDefault(p => string.Equals(p.Account, accountName, StringComparison.OrdinalIgnoreCase));
        if (playerAccount == null) return 0;
        var phaseIndex = GetIndexFromPhase(phaseName);
        if (phaseIndex < 0) return 0;
        var phase = _log.Phases.First(x => x.Name == phaseName);
        var targetIndex = phase.Targets.First();

        var damageNumbers = playerAccount.TargetDamage1S[targetIndex][phaseIndex];
        var cumulDamage = 0;
        if (time / 1000 > damageNumbers.Count)
        {
            cumulDamage = damageNumbers.Last();
        }
        else
        {
            cumulDamage = damageNumbers[(int)(time / 1000)];
        }
        if(cumulative)
        {
            return cumulDamage;
        }
        return cumulDamage / (time / 1000);

        JsonNPC[] target;
        if (phaseName == "Full Fight" && allTarget)
        {
            target = _log.Targets.ToArray();
        }
        else
        {
            var targetIds = _log.Phases?.FirstOrDefault(p => string.Equals(p.Name, phaseName, StringComparison.OrdinalIgnoreCase))?.Targets.ToArray() ?? [];
            target = _log.Targets.Where(t => targetIds.Contains(t.Id)).ToArray();
        }
        var phaseEnd = time == 0 ? GetPhaseEnd(phaseName) : GetPhaseStart(phaseName) + time;
        return GetPlayerDps(accountName, GetPhaseStart(phaseName), phaseEnd, target, cumulative: cumulative, defiance: defiance, damageType: damageType);
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
            throw new NotImplementedException();
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

    private List<(double, double)> GetBoonTimedEvents(string target, string boonName, long start, long end, string source = "")
    {
        var boonID = GetBuffIdFromBoonName(boonName);
        if (boonID == 0) return [];
        if (_log.BuffMap == null) return [];
        var player = _log.Players?.FirstOrDefault(p => string.Equals(p.Account, target, StringComparison.OrdinalIgnoreCase));
        if (player == null) return [];
        var buff = player.BuffUptimes?.FirstOrDefault(b => b.Id == boonID);
        if (buff == null) return [];


        var boonTimedEvents = new List<(double, double)>();
        boonTimedEvents.Add((start, GetBoonAtTime(target, boonName, start)));

        var prevBoon = 0;
        foreach(var buffState in buff.States)
        {
            if (buffState[0] < start || buffState[0] > end) continue;
            boonTimedEvents.Add((buffState[0], buffState[1]));
        }

        boonTimedEvents.Add((end, GetBoonAtTime(target, boonName, end)));
        return boonTimedEvents;
    }

    public double GetBoon(int group, string boonName, string phaseName = "", long time = 0, bool duration = false, bool ignoreKite = false)
    {
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
        throw new NotImplementedException("Shockwave stability check not implemented.");
    }

    public bool HasBoonDuringTime(string target, string boonName, long start, long end)
    {
        // Not directly supported in JSON, requires custom logic
        throw new NotImplementedException("Boon during time check not implemented.");
    }

    public bool IsAlive(string player, long time)
    {
        // Not directly supported in JSON, requires custom logic
        throw new NotImplementedException("Alive check not implemented.");
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
        // Not directly supported in JSON, requires custom logic
        throw new NotImplementedException("Stealth result extraction from JSON not implemented.");
    }

    public StealthTimelineCollection GetStealthTimeline()
    {
        // Not directly supported in JSON, requires custom logic
        throw new NotImplementedException("Stealth timeline extraction from JSON not implemented.");
    }

    public long GetStealthTiming(string phase)
    {
        // Not directly supported in JSON, requires custom logic
        throw new NotImplementedException("Stealth timing extraction from JSON not implemented.");
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
        return _log.Mechanics.Select(m => m.Name ?? string.Empty).ToArray();
    }

    public (string, long)[] GetMechanicLogsForPlayer(string accountName, string mechanicName, string phaseName = "", long start = 0, long end = 0)
    {
        return GetMechanicLogs(mechanicName, phaseName, start, end)
            .Where(x => string.Equals(x.Item1, accountName, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    public (string, long)[] GetMechanicLogs(string mechanicName, string phaseName = "", long start = 0, long end = 0)
    {
        var mechanics = _log.Mechanics;
        if (mechanics == null) return [];
        var mechs = mechanics.First(x => x.FullName.Equals(mechanicName));
        if (mechs == null) return [];
        return mechs.MechanicsData
            .Select(x => (x.Actor ?? "", x.Time))
            .ToArray();
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
        // Not directly supported in JSON, requires custom logic
        return Array.Empty<string>();
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
        // Not directly supported in JSON, requires custom logic
        return false;
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
            .Where(x => x.Actor == playerObject.Account)
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
