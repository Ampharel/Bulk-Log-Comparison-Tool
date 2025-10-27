using System;
using System.IO;
using BLCTWeb.Models;

namespace BLCTWeb.Stores
{
    public class RankStore : PersistentMap<ulong, Rank>
    {
        public RankStore(string? filePath = null)
            : base(ComputePath(filePath))
        {
        }

        protected override bool AutoSave => true;

        private static string ComputePath(string? provided)
        {
            if (!string.IsNullOrWhiteSpace(provided))
                return provided;

            var baseDir = AppContext.BaseDirectory ?? Directory.GetCurrentDirectory();
            return Path.Combine(baseDir, "ranks.json");
        }
    }
}