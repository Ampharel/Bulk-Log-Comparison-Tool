using GW2EIEvtcParser.ParserHelpers;
using GW2EIEvtcParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Log_Comparison_Tool.DataClasses
{
    public interface IEvtcParser
    {
        public IParsedEvtcLog ParseLog(string filePath);
        public IParsedEvtcLog ParseLog(Stream fileStream, string fileName);

    }
}
