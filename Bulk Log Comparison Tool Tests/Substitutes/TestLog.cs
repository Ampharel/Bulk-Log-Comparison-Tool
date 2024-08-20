using Bulk_Log_Comparison_Tool.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Log_Comparison_Tool_Tests.Substitutes
{
    internal class TestLog : IParsedEvtcLog
    {
        private string _logName;
        private string _logStart;
        public TestLog(string logName, string LogStart = "")
        {
            _logName = logName;
            _logStart = LogStart;
        }

        public string GetFileName()
        {
            return _logName;
        }

        public string GetLogStart()
        {
            return _logStart;
        }

        public float GetPlayerDps(string accountName, string phaseName = "")
        {
            throw new NotImplementedException();
        }
    }
}
