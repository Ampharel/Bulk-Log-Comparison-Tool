using Bulk_Log_Comparison_Tool.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Log_Comparison_Tool_Tests.Substitutes
{
    internal class TestParser : IEvtcParser
    {
        private IParsedEvtcLog? _parsedLog = null;
        public IParsedEvtcLog ParseLog(string filePath)
        {
            _parsedLog = new TestLog(filePath);
            return _parsedLog;
        }
        public IParsedEvtcLog ParseLog(string filePath, string LogStart)
        {
            _parsedLog = new TestLog(filePath, LogStart);
            return _parsedLog;
        }
    }
}
