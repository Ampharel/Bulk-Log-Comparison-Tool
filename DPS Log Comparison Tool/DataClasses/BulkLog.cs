using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Log_Comparison_Tool.DataClasses
{
    public class BulkLog
    {
        private List<IParsedEvtcLog> _logs;
        private readonly object _lock = new();

        public List<IParsedEvtcLog> Logs
        {
            get
            {
                lock (_lock)
                {
                    return _logs.OrderBy(x => x.GetFileName()).ToList();
                }
            }
        }
        private bool _stealthParsed = false;
        private Dictionary<string, Dictionary<string, string>> _stealthData = new();

        public BulkLog()
        {
            _logs = new();
        }

        public BulkLog(List<IParsedEvtcLog> logs)
        {
            _logs = logs;
        }

        public void AddLog(IParsedEvtcLog log)
        {
            lock (_lock)
            {
                _logs.Add(log);
                _stealthParsed = false;
            }
        }

        public IParsedEvtcLog? GetLog(string logName)
        {
            lock (_lock)
            {
                return _logs.FirstOrDefault(x => x.GetFileName() == logName);
            }
        }

        public void RemoveLog(string log)
        {
            lock (_lock)
            {
                _logs.RemoveAll(x => log.StartsWith(x.GetFileName()));
                _stealthParsed = false;
            }
        }

        public int[] GetGroups()
        {
            return Logs.SelectMany(x => x.GetGroups()).Distinct().ToArray();
        }

        public string[] GetPlayers()
        {
            return Logs.SelectMany(x => x.GetPlayers()).Distinct().ToArray();
        }

        public string[] GetPhases(string[] filter, bool exclusion = true)
        {
            return Logs.SelectMany(x => x.GetPhases(filter,exclusion)).Distinct().ToArray();
        }

        public bool IsPlayerInGroup(string player, int group)
        {
            return Logs.Any(x => x.IsPlayerInGroup(player, group));
        }

        public string[] GetStealthPhases()
        {
            ParseStealthData(Enums.StealthAlgoritmns.Timing);
            return _stealthData.Values.SelectMany(x => x.Keys).Distinct().ToArray();
        }

        public string[] GetMechanicNames(string phaseName = "", long start = 0, long end = 0)
        {
            return Logs.SelectMany(x => x.GetMechanicNames(phaseName, start, end)).Distinct().ToArray();
        }

        //public List<string> GetMechanicLogs(string mechanicName, string phaseName = "", long start = 0, long end = 0)
        //{
        //    var result = Logs.SelectMany(x => x.GetMechanicLogs(mechanicName, phaseName, start, end)).ToList();
        //    return [""];
        //}

        public string[] GetBoonNames()
        {
            return Logs.SelectMany(x => x.GetBoonNames()).Distinct().ToArray();
        }

        public string GetStealthResult(string logName, string phase, Enums.StealthAlgoritmns algoritmn)
        {
            ParseStealthData(algoritmn);
            if (!_stealthData.ContainsKey(logName) || !_stealthData[logName].ContainsKey(phase))
                {
                    return "";
                }
            return _stealthData[logName][phase];
        }

        private Enums.StealthAlgoritmns _algorithmnUsed = Enums.StealthAlgoritmns.Timing;

        private void ParseStealthData(Enums.StealthAlgoritmns algoritmn)
        {
            if(_stealthParsed && _algorithmnUsed == algoritmn)
            {
                return;
            }
            _algorithmnUsed = algoritmn;
            _stealthParsed = true;
            var Players = GetPlayers();
            foreach (var log in Logs)
            {
                _stealthData[log.GetFileName()] = new Dictionary<string, string>();
                foreach (var player in Players)
                {
                    var stealthData = log.GetStealthResult(player, algoritmn);
                    foreach (var data in stealthData)
                    {
                        if (!_stealthData[log.GetFileName()].ContainsKey(data.Item1))
                        {
                            _stealthData[log.GetFileName()][data.Item1] = "";
                        }
                        _stealthData[log.GetFileName()][data.Item1] += data.Item2;
                    }
                }
            }
        }
    }
}
