using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool.LibraryClasses;
using GW2EIBuilders;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIGW2API;
using System.Numerics;
using System.Text;
using static GW2EIEvtcParser.ParserHelper;

namespace Bulk_Log_Comparison_Tool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var Parser = new LogParser(new LibraryParser(false));
            CsvBuilder csvBuilder = new();
            csvBuilder.CsvString(Parser.BulkLog, $"CSV/{DateTime.Now.ToString("yyyyMMdd-HHmmss")}");
            //Console.WriteLine("Press any key to exit");
            //Console.ReadKey();
        }
    }
}
