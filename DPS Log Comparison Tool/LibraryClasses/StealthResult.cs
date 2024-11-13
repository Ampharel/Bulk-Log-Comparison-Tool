using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bulk_Log_Comparison_Tool.LibraryClasses
{
    public class StealthTimeline
    {
        public Dictionary<string, List<StealthResult>> _stealthResultsPerPhase;
        public IReadOnlyDictionary<string, List<StealthResult>> Results => _stealthResultsPerPhase.AsReadOnly();

        public StealthTimeline(Dictionary<string, List<StealthResult>> stealthResultsPerPhase)
        {
            _stealthResultsPerPhase = stealthResultsPerPhase;
        }

        public List<StealthResult> GetStealthResults(string phaseName)
        {
            if (_stealthResultsPerPhase.TryGetValue(phaseName, out List<StealthResult> value))
            {
                return value;
            }
            return new List<StealthResult>();
        }   
    }

    public class StealthResult
    {
        public readonly string Player;
        public readonly string Reason;
        public readonly long Time;
        public readonly long StealthTime;

        public StealthResult(string player, string reason, long time, long stealthTime)
        {
            Player = player;
            Reason = reason;
            Time = time;
            StealthTime = stealthTime;
        }

        public StealthResult(string player)
        {
            Player = player;
            Reason = "";
            Time = -1;
            StealthTime = -1;
        }

        public StealthResult(string player, string reason)
        {
            Player = player;
            Reason = reason;
            Time = -1;
            StealthTime = -1;
        }
    }
}
