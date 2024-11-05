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
        private EvtcParser _parser;
        private bool _multiThreadAccelerationForBuffs = true;

        internal readonly string SkillAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/SkillList.json";
        internal readonly string SpecAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/SpecList.json";
        internal readonly string TraitAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/TraitList.json";

        internal EvtcParserSettings parserSettings;
        internal GW2APIController APIController;

        public LibraryParser(bool multiThreadAccelerationForBuffs)
        {
            _multiThreadAccelerationForBuffs = multiThreadAccelerationForBuffs;

            parserSettings = new EvtcParserSettings(false, false, true, true, true, 2200, true);
            Console.WriteLine("SkillAPI: " + SkillAPICacheLocation);
            APIController = new GW2APIController(SkillAPICacheLocation, SpecAPICacheLocation, TraitAPICacheLocation);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _parser = new EvtcParser(parserSettings, APIController);
        }
        public IParsedEvtcLog ParseLog(ParserController operation, FileInfo evtc, out ParsingFailureReason parsingFailureReason, bool multiThreadAccelerationForBuffs = false)
        {
            return new ParsedLibraryLog(_parser.ParseLog(operation, evtc, out parsingFailureReason, multiThreadAccelerationForBuffs), evtc.Name);
        }
        public IParsedEvtcLog ParseLog(ParserController operation, string Name, Stream fileStream, out ParsingFailureReason parsingFailureReason, bool multiThreadAccelerationForBuffs = false)
        {
            return new ParsedLibraryLog(_parser.ParseLog(operation, fileStream, out parsingFailureReason, multiThreadAccelerationForBuffs), Name);
        }

        public IParsedEvtcLog ParseLog(string filePath)
        {
            _parser = new EvtcParser(parserSettings, APIController);
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
            _parser = new EvtcParser(parserSettings, APIController);
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
