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
        protected double GetPlayerDps(string accountName, long start, long end, AbstractSingleActor[] targets, bool cumulative = false, bool defiance = false, DamageTyping damageType = DamageTyping.All);
        public string[] GetPhases();
        public void AddPhase(string name, long start, long duration);
        public string[] GetTargets(string phaseName = "");
        public double GetBoon(string target, string boonName, string phaseName = "", long time = 0, bool duration = false);
        public List<(double,double)> GetBoonTimedEvents(string target, string boonName, string phaseName = "");
        public double GetBoon(int group, string boonName, string phaseName = "", long time = 0, bool duration = false);
        public long[] GetShockwaves(int shockwaveType);
        public bool HasPlayer(string accountName);
        public bool HasStabDuringShockwave(string player, ShockwaveType type, long shockwaveTime, out long intersectionTime);
        public bool HasBoonDuringTime(string target, string boonName, long start, long end);
        public bool IsAlive(string player, long time);
        public BuffStackTyping GetBoonStackType(string boonName);
        public List<(string, string)> GetStealthResult(string accountName, StealthAlgoritmns algoritmn, bool showLate = false);
        public StealthTimelineCollection GetStealthTimeline();
        public long GetStealthTiming(string phase);
        public List<string> GetDownReasons(string accountName);

        public string[] GetPlayers();
        public string GetSpec(string accountName);
        public int[] GetGroups();
        public bool IsPlayerInGroup(string accountName, int group);
        IEnumerable<string> GetBoonNames();
        public string[] GetMechanicNames(string phaseName = "", long start = 0, long end = 0);
        public (string, long)[] GetMechanicLogs(string mechanicName, string phaseName = "", long start = 0, long end = 0);
        public long GetPhaseStart(string phase);
        public long GetPhaseEnd(string phase);
        public string[] GetStealthPhases();
    }
}
