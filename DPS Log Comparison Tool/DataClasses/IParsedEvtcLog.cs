using Bulk_Log_Comparison_Tool.Util;
using Bulk_Log_Comparison_Tool.Enums;

namespace Bulk_Log_Comparison_Tool.DataClasses
{
    public interface IParsedEvtcLog
    {
        public string GetFileName();
        public string GetLogStart();
        public int GetPlayerDps(string accountName, string phaseName = "", DamageTyping damageType = DamageTyping.All);
        public int GetPlayerDps(string accountName, long start, long end, string[] targetNames, DamageTyping damageType = DamageTyping.All);
        public string[] GetPhases();
        public void AddPhase(string name, long start, long duration);
        public string[] GetTargets(string phaseName = "");
        public double GetBoon(string target, string boonName, string phaseName = "");
        public double GetBoon(string target, string boonName, long start, long end);
        public double GetBoon(int group, string boonName, string phaseName = "");
        public double GetBoon(int group, string boonName, long start, long end);
        public long[] GetShockwaves(int shockwaveType);
        public bool HasBoonDuringTime(string target, string boonName, long start, long end);
        public bool IsAlive(string player, long time);
        public BuffStackTyping GetBoonStackType(string boonName);
        public List<(string, string)> GetStealthResult(string accountName);

        public string[] GetPlayers();
        public int[] GetGroups();
        IEnumerable<string> GetBoonNames();
        public string[] GetMechanicNames(string phaseName = "", long start = 0, long end = 0);
        public (string, long)[] GetMechanicLogs(string mechanicName, string phaseName = "", long start = 0, long end = 0);
        public long GetPhaseStart(string phase);
        public long GetPhaseEnd(string phase);
    }
}
