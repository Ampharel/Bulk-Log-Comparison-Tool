using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Log_Comparison_Tool.DataClasses
{
    public class LastLaugh
    {
        public long SkillID { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public long Time { get; set; }
        public long HealthDamage { get; set; }
        public long ShieldDamage { get; set; }

        public LastLaugh(long skillID, string source, string target, long time, long healthDamage, long shieldDamage)
        {
            SkillID = skillID;
            Source = source;
            Target = target;
            Time = time;
            HealthDamage = healthDamage;
            ShieldDamage = shieldDamage;
        }
    }
}
