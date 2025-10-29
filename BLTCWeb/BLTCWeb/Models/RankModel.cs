using System;

namespace BLCTWeb.Models
{
    public enum InstanceType
    {
        Unknown = 0,
        HTCM = 1,
        ToFCM = 2,
        DecimaCM = 3,
        GreerCM = 4,
        UraCM = 5,
        UraLM = 6,
        Other = 99
    }

    public sealed class Rank
    {
        // Use DiscordRoleId as the identifier/key for persistence
        public ulong DiscordRoleId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Phase { get; set; } = string.Empty;

        // Required boss health percentage to reach within the phase (0..100)
        public double BossHealthPercent { get; set; }

        // Optional: boss must reach at least this many stacks of the named buff during the phase
        public string? BossBuffName { get; set; }
        public int BossBuffStackThreshold { get; set; } = 0;

        public InstanceType InstanceType { get; set; } = InstanceType.Unknown;

        // Higher value means higher rank when picking the best eligible rank
        public int Priority { get; set; } = 0;
    }
}