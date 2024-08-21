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

        public List<IParsedEvtcLog> Logs => _logs;
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
            _logs.Add(log);
            _stealthParsed = false;
        }

        public void RemoveLog(string log)
        {
            _logs.RemoveAll(x => x.GetFileName() == log);
            _stealthParsed = false;
        }

        public int[] GetGroups()
        {
            return _logs.SelectMany(x => x.GetGroups()).Distinct().ToArray();
        }

        public string[] GetPlayers()
        {
            return _logs.SelectMany(x => x.GetPlayers()).Distinct().ToArray();
        }

        public string[] GetPhases()
        {
            return _logs.SelectMany(x => x.GetPhases()).Distinct().ToArray();
        }


        public string[] GetStealthPhases()
        {
            ParseStealthData();
            return _stealthData.Values.SelectMany(x => x.Keys).Distinct().ToArray();
        }

        public string[] GetBoonNames()
        {
            return _logs.SelectMany(x => x.GetBoonNames()).Distinct().ToArray();
        }

        public string GetStealthResult(string logName, string phase)
        {
            ParseStealthData();
            if (!_stealthData.ContainsKey(logName) || !_stealthData[logName].ContainsKey(phase))
                {
                    return "";
                }
            return _stealthData[logName][phase];
        }

        private void ParseStealthData()
        {
            if(_stealthParsed)
            {
                return;
            }
            _stealthParsed = true;
            var Players = GetPlayers();
            foreach (var log in _logs)
            {
                _stealthData[log.GetFileName()] = new Dictionary<string, string>();
                foreach (var player in Players)
                {
                    var stealthData = log.GetStealthResult(player);
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
