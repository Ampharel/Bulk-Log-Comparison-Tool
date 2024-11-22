using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool.Enums;
using Bulk_Log_Comparison_Tool.LibraryClasses;

namespace BLCTWeb
{
    public class ServerParser
    {
        public StealthAlgoritmns stealthAlgoritmn = StealthAlgoritmns.OutlierFiltering;
        private BulkLog _bulklog = new();

        public BulkLog BulkLog => _bulklog;
        private List<(string, long, long)> _customPhases = new();

        public event Action NewDataEvent; 

        public void AddCustomPhase(string name, long start, long duration)
        {
            _customPhases.Add((name, start, duration));
            foreach (var log in _bulklog.Logs)
            {
                log.AddPhase(name, start, duration);
            }
            NewDataEvent();
        }

        public void AddLog(string file)
        {
            var log = new LibraryParser(false).ParseLog(file);
            foreach (var phase in _customPhases)
            {
                log.AddPhase(phase.Item1, phase.Item2, phase.Item3);
            }
            _bulklog.AddLog(log);

            NewDataEvent?.Invoke();
        }
        public void AddLog(Stream file, string name)
        {
            var log = new LibraryParser(false).ParseLog(file, name);
            foreach (var phase in _customPhases)
            {
                log.AddPhase(phase.Item1, phase.Item2, phase.Item3);
            }
            _bulklog.AddLog(log);
            NewDataEvent?.Invoke();
        }

        public void RemoveLog(string file)
        {
            _bulklog.RemoveLog(file);
            NewDataEvent?.Invoke();
        }

        public void RemoveAll()
        {
            _bulklog = new();
            NewDataEvent?.Invoke();
        }
    }
}
