using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool.Enums;
using Bulk_Log_Comparison_Tool.LibraryClasses;
using Bulk_Log_Comparison_Tool.Util;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;
using System;
using System.Collections.Generic;
using System.Linq;

public class ParsedJsonLog : IParsedEvtcLog
{
    private readonly JsonLog _log;
    private readonly string _fileName;
    private Dictionary<string, (long, long)> _customPhases = new();
    private Dictionary<string, string>? _expectedStealthPhases = new();

    public ParsedJsonLog(JsonLog log, string fileName)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));
        _fileName = fileName;
    }

    public string GetFileName() => _fileName;

    public string GetLogStart() => _log.TimeStart ?? string.Empty;

    public double GetPlayerDps(string accountName, string phaseName = "", bool allTarget = false, bool cumulative = false, bool defiance = false, DamageTyping damageType = DamageTyping.All)
    {
        throw new NotImplementedException("Playerdps from JSON not implemented.");
    }

    public double GetPlayerDps(string accountName, long time = 0, string phaseName = "", bool allTarget = false, bool cumulative = false, bool defiance = false, DamageTyping damageType = DamageTyping.All)
    {
        throw new NotImplementedException("Playerdps from JSON not implemented.");
    }

    public double GetPlayerDps(string accountName, long start, long end, SingleActor[] targets, bool cumulative = false, bool defiance = false, DamageTyping damageType = DamageTyping.All)
    {
        throw new NotImplementedException("Playerdps from JSON not implemented.");
    }

    public string[] GetPhases(string[] filter, bool exclusion = true)
    {
        if (_log.Phases == null) return Array.Empty<string>();
        var phases = _log.Phases.Select(p => p.Name ?? string.Empty);
        if (filter == null || filter.Length == 0) return phases.ToArray();
        return exclusion
            ? phases.Where(x => filter.All(y => !x.Contains(y))).ToArray()
            : phases.Where(x => filter.Any(y => x.Contains(y))).ToArray();
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
        // Not directly supported in JSON, requires custom logic
        throw new NotImplementedException("Boon extraction from JSON not implemented.");
    }

    public double GetBoonAtTime(string target, string boonName, long time)
    {
        // Not directly supported in JSON, requires custom logic
        throw new NotImplementedException("Boon at time extraction from JSON not implemented.");
    }

    public List<(double, double)> GetBoonTimedEvents(string target, string boonName, string phaseName = "", string source = "")
    {
        // Not directly supported in JSON, requires custom logic
        throw new NotImplementedException("Boon timed events extraction from JSON not implemented.");
    }

    public double GetBoon(int group, string boonName, string phaseName = "", long time = 0, bool duration = false, bool ignoreKite = false)
    {
        // Not directly supported in JSON, requires custom logic
        throw new NotImplementedException("Group boon extraction from JSON not implemented.");
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

    public List<(string, long)> GetDownReasons(string accountName)
    {
        // Not directly supported in JSON, requires custom logic
        throw new NotImplementedException("Down reasons extraction from JSON not implemented.");
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
        return _log.BuffMap.Values.Select(b => b.Name ?? string.Empty);
    }

    public string[] GetMechanicNames(string phaseName = "", long start = 0, long end = 0)
    {
        if (_log.Mechanics == null) return Array.Empty<string>();
        return _log.Mechanics.Select(m => m.Name ?? string.Empty).ToArray();
    }

    public (string, long)[] GetMechanicLogsForPlayer(string accountName, string mechanicName, string phaseName = "", long start = 0, long end = 0)
    {
        // Not directly supported in JSON, requires custom logic
        throw new NotImplementedException("Mechanic logs for player extraction from JSON not implemented.");
    }

    public (string, long)[] GetMechanicLogs(string mechanicName, string phaseName = "", long start = 0, long end = 0)
    {
        // Not directly supported in JSON, requires custom logic
        throw new NotImplementedException("Mechanic logs extraction from JSON not implemented.");
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

    public string[] GetFood(string accountName)
    {
        // Not directly supported in JSON, requires custom logic
        return Array.Empty<string>();
    }

    public string[] GetEnhancements(string account)
    {
        // Not directly supported in JSON, requires custom logic
        return Array.Empty<string>();
    }

    public bool HasReinforcedArmor(string accountName)
    {
        // Not directly supported in JSON, requires custom logic
        return false;
    }

    public List<LastLaugh> GetLastLaughs(string accountName, string phaseName)
    {
        // Not directly supported in JSON, requires custom logic
        return new List<LastLaugh>();
    }

    public List<LastLaugh> GetChampionLastLaugh(string accountName, string phaseName)
    {
        // Not directly supported in JSON, requires custom logic
        return new List<LastLaugh>();
    }

    public long[] GetZhaitanFearTimings()
    {
        // Not directly supported in JSON, requires custom logic
        return Array.Empty<long>();
    }

    public (string, long) GetCleanseReactionTime(string player, long fearTime)
    {
        // Not directly supported in JSON, requires custom logic
        return (string.Empty, 0);
    }

    public string[] GetDamageReductionsAtTime(string player, long fearTime)
    {
        // Not directly supported in JSON, requires custom logic
        return Array.Empty<string>();
    }

    public long GetBoonStripDuringPhase(string player, string phase)
    {
        // Not directly supported in JSON, requires custom logic
        return 0;
    }
}
