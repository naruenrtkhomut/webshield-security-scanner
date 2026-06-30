using WebShield.Core.Models;
using WebShield.Reporting;

namespace WebShield.Core.Tests;

public sealed class JsonReportWriterTests
{
    [Fact]
    public void Write_ShouldIncludeTargetScoreAndFindingSeverity()
    {
        var result = new ScanResult
        {
            Target = new Uri("https://example.com"),
            FinishedAt = DateTimeOffset.UtcNow
        };

        result.Findings.Add(new Finding(
            "test-check",
            "Test finding",
            Severity.High,
            "Description",
            "Impact",
            "Recommendation",
            "Evidence"));

        var json = JsonReportWriter.Write(result);

        Assert.Contains("\"target\": \"https://example.com/\"", json);
        Assert.Contains("\"score\": 75", json);
        Assert.Contains("\"severity\": \"High\"", json);
        Assert.Contains("\"checkId\": \"test-check\"", json);
    }
}
