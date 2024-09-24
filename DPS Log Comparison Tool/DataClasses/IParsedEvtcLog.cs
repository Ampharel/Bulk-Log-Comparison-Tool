using Bulk_Log_Comparison_Tool.Util;
using Bulk_Log_Comparison_Tool.Enums;
using GW2EIEvtcParser.EIData;

namespace Bulk_Log_Comparison_Tool.DataClasses
{
    public interface IParsedEvtcLog
    {
        public string GetFileName();
        public string GetLogStart();
        public double GetPlayerDps(string accountName, string phaseName = "", bool cumulative = false, bool allTarget = false, bool defiance = false, DamageTyping damageType = DamageTyping.All);
        protected double GetPlayerDps(string accountName, long start, long end, AbstractSingleActor[] targets, bool cumulative = false, bool defiance = false, DamageTyping damageType = DamageTyping.All);
        public string[] GetPhases();
        public void AddPhase(string name, long start, long duration);
        public string[] GetTargets(string phaseName = "");
        public double GetBoon(string target, string boonName, string phaseName = "", long time = 0, bool duration = false);
        public double GetBoon(int group, string boonName, string phaseName = "", long time = 0, bool duration = false);
        public long[] GetShockwaves(int shockwaveType);
        public bool HasPlayer(string accountName);
        public bool HasBoonDuringTime(string target, string boonName, long start, long end);
        public bool IsAlive(string player, long time);
        public BuffStackTyping GetBoonStackType(string boonName);
        public List<(string, string)> GetStealthResult(string accountName, StealthAlgoritmns algoritmn, bool showLate = false);
        public List<string> GetDownReasons(string accountName);

        public string[] GetPlayers();
        public int[] GetGroups();
        IEnumerable<string> GetBoonNames();
        public string[] GetMechanicNames(string phaseName = "", long start = 0, long end = 0);
        public (string, long)[] GetMechanicLogs(string mechanicName, string phaseName = "", long start = 0, long end = 0);
        public long GetPhaseStart(string phase);
        public long GetPhaseEnd(string phase);
    }
}
