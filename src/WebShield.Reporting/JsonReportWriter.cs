using System.Text.Json;
using System.Text.Json.Serialization;
using WebShield.Core.Models;

namespace WebShield.Reporting;

public static class JsonReportWriter
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public static string Write(ScanResult result)
    {
        var document = new JsonScanReport(
            result.Target.ToString(),
            result.StartedAt,
            result.FinishedAt,
            result.Score,
            result.Findings
                .GroupBy(f => f.Severity)
                .OrderByDescending(g => g.Key)
                .ToDictionary(g => g.Key.ToString(), g => g.Count()),
            result.Findings
                .OrderByDescending(f => f.Severity)
                .ThenBy(f => f.Title)
                .Select(f => new JsonFinding(
                    f.CheckId,
                    f.Title,
                    f.Severity,
                    f.Description,
                    f.Impact,
                    f.Recommendation,
                    f.Evidence))
                .ToArray());

        return JsonSerializer.Serialize(document, Options);
    }

    private sealed record JsonScanReport(
        string Target,
        DateTimeOffset StartedAt,
        DateTimeOffset FinishedAt,
        int Score,
        IReadOnlyDictionary<string, int> Summary,
        IReadOnlyCollection<JsonFinding> Findings);

    private sealed record JsonFinding(
        string CheckId,
        string Title,
        Severity Severity,
        string Description,
        string Impact,
        string Recommendation,
        string Evidence);
}
