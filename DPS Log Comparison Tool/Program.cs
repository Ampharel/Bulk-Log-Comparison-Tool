using GW2EIBuilders;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIGW2API;
using System.Numerics;
using System.Text;
using static GW2EIEvtcParser.ParserHelper;

namespace DPS_Log_Comparison_Tool
{
    static class util
    {
        public static int TryParse(this string? Source) => int.TryParse(Source, out int result) ? result : 0;
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            LogParser logParser = new();
            CsvBuilder csvBuilder = new();
            csvBuilder.CsvString(logParser.LogFiles, $"CSV/{DateTime.Now.ToString("yyyyMMdd-HHmmss")}");

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
