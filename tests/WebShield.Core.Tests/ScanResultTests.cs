using WebShield.Core.Models;

namespace WebShield.Core.Tests;

public sealed class ScanResultTests
{
    [Fact]
    public void Score_ShouldApplySeverityPenalty()
    {
        var result = new ScanResult
        {
            Target = new Uri("https://example.com"),
            FinishedAt = DateTimeOffset.UtcNow
        };

        result.Findings.Add(new Finding(
            "test",
            "High risk finding",
            Severity.High,
            "Description",
            "Impact",
            "Recommendation",
            "Evidence"));

        result.Findings.Add(new Finding(
            "test",
            "Low risk finding",
            Severity.Low,
            "Description",
            "Impact",
            "Recommendation",
            "Evidence"));

        Assert.Equal(70, result.Score);
    }
}
