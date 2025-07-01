using Xunit;
using Bulk_Log_Comparison_Tool.DataClasses;
using Bulk_Log_Comparison_Tool.LibraryClasses;

public class ParsedLogFixture
{
    public IParsedEvtcLog Log { get; }
    public ParsedLogFixture()
    {
        var baseDir = AppContext.BaseDirectory;
        var zevtcPath = System.IO.Path.Combine(baseDir, "TestData", "20250510-122619.zevtc");
        LibraryParser logParser = new LibraryParser(false);
        Log = logParser.ParseLog(zevtcPath);
    }
}

public class LibraryParsedEvtcLogTests : ParsedEvtcLogTestsBase, IClassFixture<ParsedLogFixture>
{
    public LibraryParsedEvtcLogTests(ParsedLogFixture fixture) : base(fixture.Log) { }

    [Fact]
    public void GetFileName_ReturnsCorrectName()
    {
        Assert.Equal("20250510-122619.zevtc", log.GetFileName());
    }


    [Fact]
    public void GetBoonTimedEvents_ReturnsList()
    {
        var events = log.GetBoonTimedEvents("Ampharel.6432", "Quickness", "Jormag");
        Assert.Equal(81, events.Count);
    }
    [Fact]
    public void GetCleanseReactionTime_ReturnsTuple()
    {
        var timings = log.GetZhaitanFearTimings();
        var result = log.GetCleanseReactionTime("Ampharel.6432", timings.First());
        Assert.Equal("Kar.7453", result.Item1);
        Assert.Equal(355, result.Item2);
    }
}
