using GW2EIEvtcParser.EIData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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


        public static List<long> GetUniqueElementsWithinTolerance(this IEnumerable<long> list, long tolerance)
        {
            // Initialize the result list
            var uniqueList = new List<long>();

            // Iterate through the sorted list
            foreach (var element in list)
            {
                // If the list is empty or the current element is not within the tolerance of the last added element, add it
                if (!uniqueList.Any() || uniqueList.Select(x => Math.Abs(x - element)).Min() > tolerance)
                {
                    uniqueList.Add(element);
                }
            }

            return uniqueList;
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
