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
}
