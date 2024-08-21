using Bulk_Log_Comparison_Tool.DataClasses;
using GW2EIEvtcParser;
using GW2EIGW2API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Log_Comparison_Tool
{
    public class LogParser
    {
        internal readonly string LogDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Logs/";

        private BulkLog _bulklog = new();

        public BulkLog BulkLog => _bulklog;
        public LogParser(IEvtcParser Parser)
        {
            if (Directory.Exists(LogDirectory))
            {
                foreach (string file in Directory.EnumerateFiles(LogDirectory, "*.zevtc", SearchOption.AllDirectories))
                {
                    var fInfo = new FileInfo(file);
                    var log = Parser.ParseLog(file);
                    _bulklog.AddLog(log);
                    Console.WriteLine($"Finished parsing {fInfo.Name}");
                }
            }
            else
            {
                Directory.CreateDirectory(LogDirectory);
                Console.WriteLine("Log directory not found, created new directoy. Please fill directory with log files");
            }
        }
    }

}
