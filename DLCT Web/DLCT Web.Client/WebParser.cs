using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool.LibraryClasses;

namespace DLCT_Web.Client
{
    public class WebParser
    {
        private BulkLog _bulklog = new();

        public BulkLog BulkLog => _bulklog;
        private List<(string, long, long)> _customPhases = new();

        public void AddCustomPhase(string name, long start, long duration)
        {
            _customPhases.Add((name, start, duration));
            foreach (var log in _bulklog.Logs)
            {
                log.AddPhase(name, start, duration);
            }
        }

        public void AddLog(string file)
        {

            var log = new LibraryParser(false).ParseLog(file);
            foreach (var phase in _customPhases)
            {
                log.AddPhase(phase.Item1, phase.Item2, phase.Item3);
            }
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
