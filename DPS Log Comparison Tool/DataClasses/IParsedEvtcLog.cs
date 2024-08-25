using GW2EIEvtcParser.ParserHelpers;
using GW2EIEvtcParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GW2EIEvtcParser.ParserHelper;
using Bulk_Log_Comparison_Tool.Util;

namespace Bulk_Log_Comparison_Tool.DataClasses
{
    public interface IParsedEvtcLog
    {
        public string GetFileName();
        public string GetLogStart();
        public int GetPlayerDps(string accountName, string phaseName = "", DamageTyping damageType = DamageTyping.All);
        public int GetPlayerDps(string accountName, long start, long end, string[] targetNames, DamageTyping damageType = DamageTyping.All);
        public string[] GetPhases();
        public string[] GetTargets(string phaseName = "");
        public double GetBoon(string target, string boonName, string phaseName = "");
        public double GetBoon(string target, string boonName, long start, long end);
        public double GetBoon(int group, string boonName, string phaseName = "");
        public double GetBoon(int group, string boonName, long start, long end);
        public bool HasBoonDuringTime(string target, string boonName, long start, long end);
        public BuffStackTyping GetBoonStackType(string boonName);
        public List<(string, string)> GetStealthResult(string accountName);

        public string[] GetPlayers();
        public int[] GetGroups();
        IEnumerable<string> GetBoonNames();
        public long GetPhaseStart(string phase);
        public long GetPhaseEnd(string phase);
    }
}
