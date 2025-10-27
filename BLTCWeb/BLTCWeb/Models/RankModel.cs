using System;

namespace BLCTWeb.Models
{
    public enum InstanceType
    {
        Unknown = 0,
        Raid = 1,
        Strike = 2,
        Fractal = 3,
        OpenWorld = 4,
        Golem = 5,
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

        public InstanceType InstanceType { get; set; } = InstanceType.Unknown;
    }
}