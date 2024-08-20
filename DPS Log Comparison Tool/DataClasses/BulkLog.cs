using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Log_Comparison_Tool.DataClasses
{
    public class BulkLog
    {
        private List<IParsedEvtcLog> _logs;

        public List<IParsedEvtcLog> Logs => _logs;

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
        }
    }
}
