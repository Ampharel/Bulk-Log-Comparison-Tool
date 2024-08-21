using Bulk_Log_Comparison_Tool.DataClasses;
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
            private IEvtcParser _parser;
            public UILogParser(IEvtcParser Parser)
            {
                _parser = Parser;
            }

            public void AddLog(string file)
            {
                var log = _parser.ParseLog(file);
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
