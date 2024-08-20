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
    internal class LibraryParser : IEvtcParser
    {
        private EvtcParser _parser;
        private bool _multiThreadAccelerationForBuffs = false;

        internal readonly string SkillAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/SkillList.json";
        internal readonly string SpecAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/SpecList.json";
        internal readonly string TraitAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/TraitList.json";

        public LibraryParser(bool multiThreadAccelerationForBuffs)
        {
            _multiThreadAccelerationForBuffs = multiThreadAccelerationForBuffs;
            EvtcParserSettings parserSettings = new EvtcParserSettings(false, false, true, true, true, 2200, true);
            var APIController = new GW2APIController(SkillAPICacheLocation, SpecAPICacheLocation, TraitAPICacheLocation);
            _parser = new EvtcParser(parserSettings, APIController);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        public IParsedEvtcLog ParseLog(ParserController operation, FileInfo evtc, out ParsingFailureReason parsingFailureReason, bool multiThreadAccelerationForBuffs = false)
        {
            return new ParsedLibraryLog(_parser.ParseLog(operation, evtc, out parsingFailureReason, multiThreadAccelerationForBuffs), evtc.Name);
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
    }
}
