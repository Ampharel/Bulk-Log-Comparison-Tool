using GW2EIEvtcParser;
using GW2EIGW2API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPS_Log_Comparison_Tool
{
    public class LogParser
    {
        private readonly EvtcParserSettings parserSettings = new EvtcParserSettings(false, false, true, true, true, 2200, true);
        internal readonly GW2APIController APIController;

        internal readonly string SkillAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/SkillList.json";
        internal readonly string SpecAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/SpecList.json";
        internal readonly string TraitAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/TraitList.json";

        internal readonly string LogDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Logs/";

        private List<(ParsedEvtcLog,string)> _logFiles = new();

        public List<(ParsedEvtcLog, string)> LogFiles
        {
            get
            {
                return _logFiles;
            }
        }
        public LogParser()
        {
            APIController = new GW2APIController(SkillAPICacheLocation, SpecAPICacheLocation, TraitAPICacheLocation);

            if (Directory.Exists(LogDirectory))
            {
                foreach (string file in Directory.EnumerateFiles(LogDirectory, "*.zevtc", SearchOption.AllDirectories))
                {
                    EvtcParser parser = new(parserSettings, APIController);
                    var fInfo = new FileInfo(file);
                    var log = parser.ParseLog(new TestOperationController(), fInfo, out GW2EIEvtcParser.ParserHelpers.ParsingFailureReason failureReason, false);
                    _logFiles.Add((log, fInfo.Name));
                    if (failureReason == null)
                    {
                        Console.WriteLine("Parsed log: " + fInfo.Name);
                    }
                    else
                    {
                        Console.WriteLine("Failed to parse log: " + fInfo.Name + " with failure reason: " + failureReason.ToString());
                    }
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
