using System.Net.Http;
using System.Text.Json;
using Bulk_Log_Comparison_Tool.DataClasses;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIJSON;


public class JsonParser : IEvtcParser
{
    private static readonly HttpClient _httpClient = new();

    public IParsedEvtcLog ParseLog(string filePath)
    {
        if (Uri.TryCreate(filePath, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            // Handle URL
            using var stream = _httpClient.GetStreamAsync(filePath).GetAwaiter().GetResult();
            return ParseJsonLog(stream, filePath);
        }
        else
        {
            // Handle local file
            using var stream = File.OpenRead(filePath);
            return ParseJsonLog(stream, Path.GetFileName(filePath));
        }
    }

    public IParsedEvtcLog ParseLog(Stream fileStream, string fileName)
    {
        return ParseJsonLog(fileStream, fileName);
    }

    private IParsedEvtcLog ParseJsonLog(Stream fileStream, string fileName)
    {
        var log = JsonSerializer.Deserialize<JsonLog>(fileStream,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    IncludeFields = true
                });

        if (log == null)
            throw new InvalidOperationException("Failed to parse JSON log.");
        return new ParsedJsonLog(log, fileName);
    }
}

