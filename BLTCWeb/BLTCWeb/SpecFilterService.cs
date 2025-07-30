using Bulk_Log_Comparison_Tool.DataClasses;
using System.Collections.Generic;
using System.Linq;

namespace BLCTWeb
{
    public class SpecFilterService
    {
        private readonly Dictionary<string, HashSet<string>> _disabledSpecs = new();

        public void Toggle(string playerId, string specId)
        {
            if (!_disabledSpecs.ContainsKey(playerId))
                _disabledSpecs[playerId] = new HashSet<string>();

            if (!_disabledSpecs[playerId].Add(specId))
                _disabledSpecs[playerId].Remove(specId);
        }

        public bool IsEnabled(string playerId, string specId)
        {
            return !_disabledSpecs.ContainsKey(playerId) || !_disabledSpecs[playerId].Contains(specId);
        }

        public IEnumerable<IParsedEvtcLog> FilterLogs(IEnumerable<IParsedEvtcLog> logs)
        {
            return logs.Where(log =>
            {
                foreach (var player in log.GetPlayers())
                {
                    var spec = log.GetSpec(player);
                    if (_disabledSpecs.TryGetValue(player, out var disabled) && disabled.Contains(spec))
                    {
                        return false;
                    }
                }
                return true;
            });
        }
    }
}
