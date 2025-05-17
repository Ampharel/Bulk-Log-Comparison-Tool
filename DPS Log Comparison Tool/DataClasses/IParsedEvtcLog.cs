using Bulk_Log_Comparison_Tool.Util;
using Bulk_Log_Comparison_Tool.Enums;
using GW2EIEvtcParser.EIData;
using Bulk_Log_Comparison_Tool.LibraryClasses;

namespace Bulk_Log_Comparison_Tool.DataClasses
{
    public interface IParsedEvtcLog
    {
        public string GetFileName();
        public string GetLogStart();
        public double GetPlayerDps(string accountName, string phaseName = "", bool allTarget = false, bool cumulative = false, bool defiance = false, DamageTyping damageType = DamageTyping.All);
        public double GetPlayerDps(string accountName, long time = 0, string phaseName = "",  bool allTarget = false, bool cumulative = false, bool defiance = false, DamageTyping damageType = DamageTyping.All);
        public string[] GetPhases(string[] filter, bool exclusion = true);
        public void AddPhase(string name, long start, long duration);
        public string[] GetTargets(string phaseName = "");
        public double GetBoon(string target, string boonName, string phaseName = "", long time = 0, bool duration = false);
        public double GetBoonAtTime(string target, string boonName, long time);
        public List<(double,double)> GetBoonTimedEvents(string target, string boonName, string phaseName = "", string source = "");
        public double GetBoon(int group, string boonName, string phaseName = "", long time = 0, bool duration = false, bool ignoreKite = false);
        public long[] GetShockwaves(int shockwaveType);
        public bool HasPlayer(string accountName);
        public bool HasStabDuringShockwave(string player, ShockwaveType type, long shockwaveTime, out long intersectionTime);
        public bool HasBoonDuringTime(string target, string boonName, long start, long end);
        public bool IsAlive(string player, long time);
        public BuffStackTyping GetBoonStackType(string boonName);
        public List<(string, string)> GetStealthResult(string accountName, StealthAlgoritmns algoritmn, bool showLate = false);
        public StealthTimelineCollection GetStealthTimeline();
        public long GetStealthTiming(string phase);
        public List<(string, long)> GetDownReasons(string accountName);

        public string[] GetPlayers();
        public string GetSpec(string accountName);
        public int[] GetGroups();
        public int GetPlayerGroup(string accountName);
        public bool IsPlayerInGroup(string accountName, int group);
        IEnumerable<string> GetBoonNames();
        public string[] GetMechanicNames(string phaseName = "", long start = 0, long end = 0);
        public (string, long)[] GetMechanicLogsForPlayer(string accountName, string mechanicName, string phaseName = "", long start = 0, long end = 0);
        public (string, long)[] GetMechanicLogs(string mechanicName, string phaseName = "", long start = 0, long end = 0);
        public long GetPhaseStart(string phase);
        public long GetPhaseEnd(string phase);
        public string[] GetStealthPhases(); 
        public string[] GetConsumables(string account);
        public bool HasReinforcedArmor(string accountName);
        public List<LastLaugh> GetLastLaughs(string accountName, string phaseName);
        public List<LastLaugh> GetChampionLastLaugh(string accountName, string phaseName);
        public long[] GetZhaitanFearTimings();
        public (string, long) GetCleanseReactionTime(string player, long fearTime);
        public string[] GetDamageReductionsAtTime(string player, long fearTime);
        public long GetBoonStripDuringPhase(string player, string phase);
    }
}
