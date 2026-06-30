namespace WebShield.Core.Models;

public sealed record Finding(
    string CheckId,
    string Title,
    Severity Severity,
    string Description,
    string Impact,
    string Recommendation,
    string Evidence);
