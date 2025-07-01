using Xunit;
using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool.LibraryClasses;

public class ParsedJsonLogFixture
{
    public IParsedEvtcLog Log { get; }
    public ParsedJsonLogFixture()
    {
        var baseDir = AppContext.BaseDirectory;
        var jsonPath = System.IO.Path.Combine(baseDir, "TestData", "20250510-122619.zevtc.json");
        JsonParser logParser = new JsonParser();
        Log = logParser.ParseLog(jsonPath);
    }
}

public class JsonParsedEvtcLogTests : ParsedEvtcLogTestsBase, IClassFixture<ParsedJsonLogFixture>
{
    public JsonParsedEvtcLogTests(ParsedJsonLogFixture fixture) : base(fixture.Log) { }

    [Fact]
    public void GetFileName_ReturnsCorrectName()
    {
        Assert.Equal("20250510-122619.zevtc.json", log.GetFileName());
    }

    [Fact]
    public void GetBoonTimedEvents_ReturnsList()
    {
        var events = log.GetBoonTimedEvents("Ampharel.6432", "Quickness", "Jormag");
        Assert.Equal(3, events.Count);
    }


    [Fact]
    public void GetCleanseReactionTime_ReturnsTuple()
    {
        var timings = log.GetZhaitanFearTimings();
        var result = log.GetCleanseReactionTime("Ampharel.6432", timings.First());
        Assert.Equal("", result.Item1);
        Assert.Equal(355, result.Item2);
    }
}
