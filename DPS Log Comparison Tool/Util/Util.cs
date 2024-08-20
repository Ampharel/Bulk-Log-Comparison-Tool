using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bulk_Log_Comparison_Tool.Util
{
    static class Util
    {
        public static int TryParse(this string? Source) => int.TryParse(Source, out int result) ? result : 0;
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
