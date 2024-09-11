using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Log_Comparison_Tool.Util
{
    public static class Util
    {
        public static int TryParse(this string? Source) => int.TryParse(Source, out int result) ? result : 0;

        public static List<int> TrimmedAverage(List<int> ints)
        {
            if (ints.Count == 0)
            {
                return ints;
            }
            var sortedInts = ints.OrderBy(x => x).ToList();
            var Max = sortedInts.Max();
            return sortedInts.Where(x => x >= Max * 0.6).ToList();
        }
        public static List<double> TrimmedAverage(List<double> doubles)
        {
            if (doubles.Count == 0)
            {
                return doubles;
            }
            var sortedDoubles = doubles.OrderBy(x => x).ToList();
            var Max = sortedDoubles.Max();
            return sortedDoubles.Where(x => x >= Max * 0.6).ToList();
        }
    }

    public enum DamageTyping
    {
        Power = 1,
        Strike = 2,
        Condition = 4,
        LifeLeech = 8,
        Barrier = 16,
        All = 31
    }
    public enum BuffStackTyping : byte
    {
        StackingConditionalLoss,
        Queue,
        StackingTargetUniqueSrc,
        Regeneration,
        Stacking,
        Force,
        Unknown
    }
}
