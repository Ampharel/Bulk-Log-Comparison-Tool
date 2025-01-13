using Bulk_Log_Comparison_Tool.DataClasses;
using GW2EIEvtcParser;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Log_Comparison_Tool.LibraryClasses
{
    public class LibraryParser : IEvtcParser
    {
        private bool _multiThreadAccelerationForBuffs = true;

        internal readonly string SkillAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/SkillList.json";
        internal readonly string SpecAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/SpecList.json";
        internal readonly string TraitAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/TraitList.json";

        internal EvtcParserSettings parserSettings;
        internal static GW2APIController? APIController;
        private object _apiLock = new object();


        public LibraryParser(bool multiThreadAccelerationForBuffs)
        {
            _multiThreadAccelerationForBuffs = multiThreadAccelerationForBuffs;

            parserSettings = new EvtcParserSettings(false, false, true, true, true, 2200, true);
            Console.WriteLine("SkillAPI: " + SkillAPICacheLocation);
            lock (_apiLock)
            {
                if (APIController == null)
                {
                    APIController = new GW2APIController(SkillAPICacheLocation, SpecAPICacheLocation, TraitAPICacheLocation);
                }
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        public IParsedEvtcLog ParseLog(ParserController operation, FileInfo evtc, out ParsingFailureReason parsingFailureReason, bool multiThreadAccelerationForBuffs = false)
        {
            return new ParsedLibraryLog(new EvtcParser(parserSettings, APIController).ParseLog(operation, evtc, out parsingFailureReason, multiThreadAccelerationForBuffs), evtc.Name);
        }
        public IParsedEvtcLog ParseLog(ParserController operation, string Name, Stream fileStream, out ParsingFailureReason parsingFailureReason, bool multiThreadAccelerationForBuffs = false)
        {
            return new ParsedLibraryLog(new EvtcParser(parserSettings, APIController).ParseLog(operation, fileStream, out parsingFailureReason, multiThreadAccelerationForBuffs), Name);
        }

        public IParsedEvtcLog ParseLog(string filePath)
        {
            var fInfo = new FileInfo(filePath);
            ParsingFailureReason parsingFailureReason;
            var Log = ParseLog(new TestOperationController(), fInfo, out parsingFailureReason, _multiThreadAccelerationForBuffs);
            if(parsingFailureReason != null)
            {
                throw new InvalidOperationException("Parsing failed: " + parsingFailureReason.Reason);
            }
            return Log;
        }
        public IParsedEvtcLog ParseLog(Stream fileStream, string fileName)
        {
            ParsingFailureReason parsingFailureReason;
            var Log = ParseLog(new TestOperationController(), fileName, fileStream, out parsingFailureReason, _multiThreadAccelerationForBuffs);
            if (parsingFailureReason != null)
            {
                Console.WriteLine("Parsing failed: " + parsingFailureReason.Reason);
                return null;
                throw new InvalidOperationException("Parsing failed: " + parsingFailureReason.Reason);
            }
            return Log;
        }
    }
}
