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
    public class StealthTimelineCollection
    {

        public Dictionary<string, StealthTimeline> _stealthResultsPerPhase;
        public IReadOnlyDictionary<string, StealthTimeline> Results => _stealthResultsPerPhase.AsReadOnly();

        public StealthTimelineCollection(Dictionary<string, StealthTimeline> stealthResultsPerPhase)
        {
            _stealthResultsPerPhase = stealthResultsPerPhase;
        }

        public StealthTimeline GetStealthResults(string phaseName)
        {
            if (_stealthResultsPerPhase.TryGetValue(phaseName, out StealthTimeline value))
            {
                return value;
            }
            return new StealthTimeline();
        }
    }
    public class StealthTimeline
    {
        public string Phase;
        public long MassInvisTime;
        public long StealthEventTime;
        public List<StealthResult> Results;

        public StealthTimeline()
        {
            Phase = "";
            MassInvisTime = -1;
            StealthEventTime = -1;
            Results = new List<StealthResult>();
        }

        public StealthTimeline(string phase, long massInvisTime, long stealthEvent, List<StealthResult> results)
        {
            Phase = phase;
            MassInvisTime = massInvisTime;
            StealthEventTime = stealthEvent;
            Results = results;
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
