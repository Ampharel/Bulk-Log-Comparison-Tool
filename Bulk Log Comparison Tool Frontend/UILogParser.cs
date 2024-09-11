using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool.LibraryClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Log_Comparison_Tool_Frontend
{
    namespace Bulk_Log_Comparison_Tool
    {
        public class UILogParser
        {
            private BulkLog _bulklog = new();

            public BulkLog BulkLog => _bulklog;
            private List<(string, long, long)> _customPhases = new();
            public UILogParser()
            {
            }

            public void AddCustomPhase(string name, long start, long duration)
            {
                _customPhases.Add((name, start, duration));
                foreach(var log in _bulklog.Logs)
                {
                    log.AddPhase(name, start, duration);
                }
            }

            public void AddLog(string file)
            {
                
                var log = new LibraryParser(false).ParseLog(file);
                foreach(var phase in _customPhases)
                {
                    log.AddPhase(phase.Item1, phase.Item2, phase.Item3);
                }
                _bulklog.AddLog(log);
            }

            public void RemoveLog(string file)
            {
                _bulklog.RemoveLog(file);
            }

            public void RemoveAll()
            {
                _bulklog = new();
            }
        }

    }

}
