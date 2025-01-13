using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool.Enums;
using Bulk_Log_Comparison_Tool.LibraryClasses;
using System.Diagnostics;

namespace BLCTWeb
{
    public class ServerParser
    {
        public StealthAlgoritmns stealthAlgoritmn = StealthAlgoritmns.OutlierFiltering;
        private BulkLog _bulklog = new();

        public BulkLog BulkLog => _bulklog;
        private List<(string, long, long)> _customPhases = new();

        public event Action NewDataEvent;
        private LibraryParser parser = new LibraryParser(false);

        public void AddCustomPhase(string name, long start, long duration)
        {
            _customPhases.Add((name, start, duration));
            foreach (var log in _bulklog.Logs)
            {
                log.AddPhase(name, start, duration);
            }
            NewDataEvent();
        }

        public bool HasLog(string file)
        {
            return _bulklog.Logs.Any(x => x.GetFileName().Equals(file));
        }

        public void AddLog(string file, bool multiload = false)
        {
            var log = new LibraryParser(false).ParseLog(file);
            foreach (var phase in _customPhases)
            {
                log.AddPhase(phase.Item1, phase.Item2, phase.Item3);
            }
            _bulklog.AddLog(log);

            if(!multiload)
                NewDataEvent?.Invoke();
        }

        public void FinishMultiload()
        {
            NewDataEvent?.Invoke();
        }

        public void AddLog(Stream file, string name, bool multiload = false)
        {
            var log = parser.ParseLog(file, name);
            foreach (var phase in _customPhases)
            {
                log.AddPhase(phase.Item1, phase.Item2, phase.Item3);
            }
            _bulklog.AddLog(log);

            if (!multiload)
                NewDataEvent?.Invoke();
        }

        public void RemoveLog(string file, bool multiload = false)
        {
            _bulklog.RemoveLog(file);

            if (!multiload)
                NewDataEvent?.Invoke();
        }

        public void RemoveAll()
        {
            _bulklog = new();
            NewDataEvent?.Invoke();
        }

        public bool IsPlayerInGroup(string player, int group)
        {
            return _bulklog.IsPlayerInGroup(player, group);
        }
    }
}
