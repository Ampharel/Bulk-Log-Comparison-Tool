using Xunit;
using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool.Enums;
using System.Linq;
using Bulk_Log_Comparison_Tool.Util;

public abstract class ParsedEvtcLogTestsBase
{
    protected readonly IParsedEvtcLog log;

    protected ParsedEvtcLogTestsBase(IParsedEvtcLog log)
    {
        this.log = log;
    }

    [Fact]
    public void GetLogStart_ReturnsExpectedStart()
    {
        var logStart = log.GetLogStart();
        // Try to parse the log start as DateTimeOffset (handles timezone if present)
        var parsed = DateTimeOffset.Parse(logStart);
        // Compare to the expected UTC time
        var expected = new DateTimeOffset(2025, 5, 10, 11, 15, 7, TimeSpan.Zero); // adjust as needed
        Assert.Equal(expected.UtcDateTime, parsed.UtcDateTime);
    }

    [Fact]
    public void GetPlayerDps_ByPhase_ReturnsExpectedDps()
    {
        double dps = log.GetPlayerDps("Ampharel.6432", "Jormag");
        Assert.InRange(dps, 51475, 51485);
    }

    [Fact]
    public void GetPlayerDps_ByTime_ReturnsExpectedDps()
    {
        double dps = log.GetPlayerDps("Ampharel.6432", 5000, "Jormag");
        Assert.InRange(dps, 51085, 51090);
    }

    [Fact]
    public void GetPlayerGoliathBurst_ByTime_ReturnsExpectedBurst()
    {
        double burst = log.GetPlayerDps("Ampharel.6432", phaseName: "Void Goliath", time: 15000, cumulative: true);
        Assert.Equal(437205, burst, 10);
    }

    [Fact]
    public void GetPhases_ReturnsExpectedPhases()
    {
        var expectedPhases = new[]
        {
            "Full Fight", "Purification 1", "Heart 1 Breakbar 1", "Jormag", "Primordus", "Kralkatorrik",
            "Void Time Caster Breakbar 1", "Purification 2", "Void Time Caster", "Heart 2 Breakbar 1",
            "Mordremoth", "Void Giant Breakbar 1", "Void Giant Breakbar 1", "Void Giant Breakbar 1",
            "Giants", "Zhaitan", "Purification 3", "Void Saltspray Dragon", "Void Saltspray Dragon Breakbar 1",
            "Heart 3 Breakbar 1", "Soo-Won", "Soo-Won 1", "Purification 4", "Void Obliterator Breakbar 1",
            "Void Goliath Breakbar 1", "Void Obliterator", "Void Goliath", "Soo-Won 2", "Void Obliterator Breakbar 2"
        };
        Assert.Equal(expectedPhases, log.GetPhases(System.Array.Empty<string>()));
    }

    [Fact]
    public void GetPhasesExclusion_ReturnsExpectedPhases()
    {
        var expectedPhases = new[]
        {
            "Full Fight", "Jormag", "Primordus", "Kralkatorrik", "Void Time Caster", "Mordremoth",
            "Giants", "Zhaitan", "Void Saltspray Dragon", "Soo-Won", "Soo-Won 1", "Void Obliterator",
            "Void Goliath", "Soo-Won 2"
        };
        Assert.Equal(expectedPhases, log.GetPhases(new[] { "Purification", "Breakbar", "|" }, true));
    }

    [Fact]
    public void GetPhasesInclusion_ReturnsExpectedPhases()
    {
        var expectedPhases = new[]
        {
            "Purification 1", "Purification 2", "Purification 3", "Purification 4"
        };
        Assert.Equal(expectedPhases, log.GetPhases(new[] { "Purification" }, false));
    }

    [Fact]
    public void AddPhase_AddsPhaseSuccessfully()
    {
        log.AddPhase("TestPhase", 0, 1000);
        Assert.Contains("TestPhase|0|1000", log.GetPhases(System.Array.Empty<string>()));
    }

    [Fact]
    public void GetTargets_ReturnsExpectedTargets()
    {
        var targets = log.GetTargets();
        string[] controlTargets = { "The JormagVoid", "The PrimordusVoid", "The KralkatorrikVoid", "The MordremothVoid", "The ZhaitanVoid", "The SooWonVoid", "Heart 4" };
        foreach(var controlTarget in controlTargets)
        {
            Assert.Contains(controlTarget, targets);
        }
    }

    [Fact]
    public void GetBoon_ByTarget_ReturnsExpectedValue()
    {
        double boon = log.GetBoon("Ampharel.6432", "Might");
        Assert.Equal(24.455, boon, 0.01);
    }

    [Fact]
    public void GetBoon_ByTarget_InPurification1_ReturnsExpectedValue()
    {
        double boon = log.GetBoon("Ampharel.6432", "Might", "Purification 1");
        Assert.Equal(25.0, boon, 0.01);
    }
    [Fact]
    public void GetBoon_ByTarget_InJormag_ReturnsExpectedValue()
    {
        double boon = log.GetBoon("Ampharel.6432", "Might", "Jormag");
        Assert.Equal(25.0, boon, 0.01);
    }
    [Fact]
    public void GetBoon_ByTarget_InPrimordus_ReturnsExpectedValue()
    {
        double boon = log.GetBoon("Ampharel.6432", "Might", "Primordus");
        Assert.Equal(23.985, boon, 0.01);
    }
    [Fact]
    public void GetBoon_ByTarget_InKralkatorrik_ReturnsExpectedValue()
    {
        double boon = log.GetBoon("Ampharel.6432", "Might", "Kralkatorrik");
        Assert.Equal(25.0, boon, 0.01);
    }
    [Fact]
    public void GetBoon_ByTarget_InPurification2_ReturnsExpectedValue()
    {
        double boon = log.GetBoon("Ampharel.6432", "Might", "Purification 2");
        Assert.Equal(25.0, boon, 0.01);
    }
    [Fact]
    public void GetBoon_ByTarget_InMordemoth_ReturnsExpectedValue()
    {
        double boon = log.GetBoon("Ampharel.6432", "Might", "Mordremoth");
        Assert.Equal(25.0, boon, 0.01);
    }
    [Fact]
    public void GetBoon_ByTarget_InZhaitan_ReturnsExpectedValue()
    {
        double boon = log.GetBoon("Ampharel.6432", "Might", "Zhaitan");
        Assert.Equal(25.0, boon, 0.01);
    }
    [Fact]
    public void GetBoon_ByTarget_InPurification3_ReturnsExpectedValue()
    {
        double boon = log.GetBoon("Ampharel.6432", "Might", "Purification 3");
        Assert.Equal(24.868, boon, 0.01);
    }
    [Fact]
    public void GetBoon_ByTarget_InSooWon1_ReturnsExpectedValue()
    {
        double boon = log.GetBoon("Ampharel.6432", "Might", "Soo-Won 1");
        Assert.Equal(25.0, boon, 0.01);
    }
    [Fact]
    public void GetBoon_ByTarget_InPurification4_ReturnsExpectedValue()
    {
        double boon = log.GetBoon("Ampharel.6432", "Might", "Purification 4");
        Assert.Equal(24.688, boon, 0.01);
    }
    [Fact]
    public void GetBoon_ByTarget_InSooWon2_ReturnsExpectedValue()
    {
        double boon = log.GetBoon("Ampharel.6432", "Might", "Soo-Won 2");
        Assert.Equal(24.259, boon, 0.01);
    }

    [Fact]
    public void GetBoonAtTime_ReturnsExpectedValue()
    {
        double boon = log.GetBoonAtTime("Ampharel.6432", "Might", 1000);
        Assert.Equal(0, boon, 0.01);
    }


    [Fact]
    public void GetBoon_ByGroup_ReturnsExpectedValue()
    {
        double boon = log.GetBoon(1, "Might");
        Assert.Equal(23.154, boon, 0.01);
    }


    [Fact]
    public void GetBoon_ByGroup_InPhase_ReturnsExpectedValue()
    {
        double boon = log.GetBoon(1, "Might", "Primordus");
        Assert.Equal(22.934, boon, 0.01);
    }

    [Fact]
    public void GetShockwaves_ReturnsArray()
    {
        var shockwaves = log.GetShockwaves(0);
        Assert.Equal(6, shockwaves.Length);
    }

    [Fact]
    public void HasPlayer_ReturnsTrueForExistingPlayer()
    {
        Assert.True(log.HasPlayer("Ampharel.6432"));
    }

    [Fact]
    public void HasPlayer_ReturnsFalseForNotExistingPlayer()
    {
        Assert.False(log.HasPlayer("Pharelam.6432"));
    }

    [Fact]
    public void HasStabDuringShockwave_ReturnsBool()
    {
        long intersectionTime;
        bool result = log.HasStabDuringShockwave("Ampharel.6432", ShockwaveType.Mordemoth, 1000, out intersectionTime);
        Assert.True(result);
    }

    [Fact]
    public void HasBoonDuringTime_ReturnsBool()
    {
        bool result = log.HasBoonDuringTime("Ampharel.6432", "Might", 0, 1000);
        Assert.IsType<bool>(result);
        Assert.True(result);
    }

    [Fact]
    public void IsAlive_ReturnsTrueWhenPlayerIsAlive()
    {
        bool result = log.IsAlive("Ampharel.6432", 1000);
        Assert.IsType<bool>(result);
        Assert.True(result);
    }

    [Fact]
    public void IsAlive_ReturnsFalseWhenPlayerIsDead()
    {
        bool result = log.IsAlive("Daniel.3196", 645735);
        Assert.IsType<bool>(result);
        Assert.False(result);
    }

    [Fact]
    public void GetBoonStackType_ReturnsEnum()
    {
        var type = log.GetBoonStackType("Might");
        Assert.IsType<BuffStackTyping>(type);
    }

    [Fact]
    public void GetBoonStackType_ReturnsStackingType()
    {
        var type = log.GetBoonStackType("Might");
        Assert.Equal(BuffStackTyping.Stacking, type);
    }

    [Fact]
    public void GetBoonStackType_ReturnsQueueType()
    {
        var type = log.GetBoonStackType("Quickness");
        Assert.Equal(BuffStackTyping.Queue, type);
    }

    [Fact]
    public void GetStealthResult_ReturnsList()
    {
        var result = log.GetStealthResult("rentrogen.2586", StealthAlgoritmns.MedianTiming);
        var giantStealth = result.First(x => x.Item1.Equals("Giants"));
        Assert.Equal("-2.884s early Grasping Shadows", giantStealth.Item2);
    }

    [Fact]
    public void GetStealthTimeline_ReturnsCollection()
    {
        var timeline = log.GetStealthTimeline();
        var giantStealth = timeline.Results["Giants"];
        var stealthResult = giantStealth.Results.First(x => x.Player == "rentrogen.2586");
        Assert.Equal("Grasping Shadows", stealthResult.Reason);
        Assert.Equal(339571, stealthResult.StealthTime);
    }

    [Fact]
    public void GetStealthTiming_ReturnsLong()
    {
        long timing = log.GetStealthTiming("Giants");
        Assert.Equal(339571, timing);
    }

    [Fact]
    public void GetDownReasons_ReturnsList()
    {
        var reasons = log.GetDownReasons("Nark.1964");
        var firstDownReason = reasons.First();
        Assert.Contains("Targeted Expulsion", firstDownReason.Item1);
        Assert.Equal(517683, firstDownReason.Item2);
    }

    [Fact]
    public void GetPlayers_ReturnsExpectedPlayers()
    {
        string[] expectedPlayers = { "rimiserk.7951", "Daniel.3196", "rentrogen.2586", "Drakh Valor.3826", "alenet.3825", "Kar.7453", "Ampharel.6432", "Bunny.9436", "Nark.1964", "Painbow.6059" };
        Assert.Equal(expectedPlayers, log.GetPlayers());
    }

    [Fact]
    public void GetSpec_ReturnsExpectedSpec()
    {
        Assert.Equal("Virtuoso", log.GetSpec("Ampharel.6432"));
    }

    [Fact]
    public void GetGroups_ReturnsExpectedGroups()
    {
        var expectedGroups = new[] { 1, 2 };
        Assert.Equal(expectedGroups, log.GetGroups());
    }

    [Fact]
    public void GetPlayerGroup_ReturnsGroup()
    {
        int group = log.GetPlayerGroup("Ampharel.6432");
        Assert.True(group == 2);
    }

    [Fact]
    public void IsPlayerInGroup_ReturnsTrueForCorrectGroup()
    {
        Assert.True(log.IsPlayerInGroup("Ampharel.6432", 2));
    }

    [Fact]
    public void IsPlayerInGroup_ReturnsFalseForIncorrectGroup()
    {
        Assert.False(log.IsPlayerInGroup("Ampharel.6432", 1));
    }

    [Fact]
    public void IsPlayerInGroup_ReturnsFalseForNotExistingGroup()
    {
        Assert.False(log.IsPlayerInGroup("Ampharel.6432", 99));
    }

    [Fact]
    public void GetBoonNames_ReturnsExpectedBoonNames()
    {
        var expectedBoons = new[] { "Might", "Fury", "Quickness", "Alacrity", "Protection", "Regeneration", "Vigor", "Stability", "Swiftness", "Resistance", "Resolution" };
        Array.Sort(expectedBoons);
        var boonArray = log.GetBoonNames().ToArray();
        Array.Sort(boonArray);
        Assert.Equal(expectedBoons, boonArray);
    }

    [Fact]
    public void GetMechanicNames_ReturnsExpectedMechanics()
    {
        Assert.Equal("Achiv Jumping Nope Ropes", log.GetMechanicNames().First());
    }

    [Fact]
    public void GetMechanicLogsForPlayer_ReturnsArray()
    {
        var logs = log.GetMechanicLogsForPlayer("Ampharel.6432", "Void Explosion", "Full Fight");
        Assert.Equal(7, logs.Count());
    }

    [Fact]
    public void GetMechanicLogs_ReturnsArray()
    {
        var logs = log.GetMechanicLogs("Void Explosion", "Full Fight");
        Assert.Equal(68, logs.Count());
    }

    [Fact]
    public void GetPhaseStart_ReturnsLong()
    {
        long start = log.GetPhaseStart("Jormag");
        Assert.IsType<long>(start);
        Assert.Equal(34592L, start);
    }

    [Fact]
    public void GetPhaseEnd_ReturnsLong()
    {
        long end = log.GetPhaseEnd("Jormag");
        Assert.IsType<long>(end);
        Assert.Equal(85591L, end);
    }

    [Fact]
    public void GetStealthPhases_ReturnsArray()
    {
        var phases = log.GetStealthPhases();
        Assert.Equal(4, phases.Count());
    }

    [Fact]
    public void GetConsumables_ReturnArray()
    {
        var consumables = log.GetConsumables("Ampharel.6432");
        string[] expected = { "Writ of Masterful Strength", "Dragon's Breath Bun", "Steamed Red Dumpling", "Block of Tofu", "Plate of Steak and Asparagus Dinner" };
        foreach (var item in expected)
        {
            Assert.Contains(item, consumables);
        }
    }


    [Fact]
    public void HasReinforcedArmor_ReturnsBool()
    {
        bool result = log.HasReinforcedArmor("Ampharel.6432");
        Assert.IsType<bool>(result);
    }

    [Fact]
    public void HasReinforcedArmor_ReturnsTrueWhenPlayerHasBuff()
    {
        bool result = log.HasReinforcedArmor("Ampharel.6432");
        Assert.True(result);
    }

    [Fact]
    public void GetLastLaughs_ReturnsList()
    {
        var laughs = log.GetLastLaughs("Ampharel.6432", "Full Fight");
        Assert.Equal(7, laughs.Count);
    }

    [Fact]
    public void GetChampionLastLaugh_ReturnsList()
    {
        var laughs = log.GetChampionLastLaugh("Drakh Valor.3826", "Full Fight");
        Assert.Empty(laughs);
    }

    [Fact]
    public void GetZhaitanFearTimings_ReturnsArray()
    {
        var timings = log.GetZhaitanFearTimings();
        Assert.Equal(2, timings.Count());
    }

    [Fact]
    public void GetDamageReductionsAtTime_ReturnsArray()
    {
        var timings = log.GetZhaitanFearTimings();
        var reductions = log.GetDamageReductionsAtTime("Ampharel.6432", timings.First());
        Assert.Single(reductions);
    }

    [Fact]
    public void GetBoonStripDuringPhase_ReturnsLong()
    {
        long result = log.GetBoonStripDuringPhase("Ampharel.6432", "Primordus");
        Assert.Equal(8, result);
    }

    [Fact]
    public void GetStealthPhases_ReturnsExpectedPhases()
    {
        var expected = new[] { "Primordus", "Kralkatorrik", "Giants", "Soo-Won 2" };
        Assert.Equal(expected, log.GetStealthPhases());
    }
    [Fact]
    public void GetPlayerDps_Defiance_ReturnsOnlyBreakbarDamage()
    {
        double cc = log.GetPlayerDps("Ampharel.6432", "Void Time Caster Breakbar 1", defiance: true, cumulative: true);
        // Should be in the expected breakbar damage range, not normal DPS
        Assert.Equal(300, cc); // Adjust range as appropriate for your data
    }
    [Fact]
    public void GetPlayerDps_FullFightAllTargets_SumsAllTargets()
    {
        double dpsSingle = log.GetPlayerDps("Ampharel.6432", "Full Fight", allTarget: true);
        Assert.Equal(31971, dpsSingle);
    }
}
