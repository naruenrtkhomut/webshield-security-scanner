namespace WebShield.Core.Models;

public sealed class ScanResult
{
    public required Uri Target { get; init; }
    public DateTimeOffset StartedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset FinishedAt { get; set; }
    public List<Finding> Findings { get; } = [];

    public int Score => CalculateScore();

    private int CalculateScore()
    {
        var penalty = Findings.Sum(f => f.Severity switch
        {
            Severity.Critical => 40,
            Severity.High => 25,
            Severity.Medium => 15,
            Severity.Low => 5,
            _ => 0
        });

        return Math.Max(0, 100 - penalty);
    }
}
