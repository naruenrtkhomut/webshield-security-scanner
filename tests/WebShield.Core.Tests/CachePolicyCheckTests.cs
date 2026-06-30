using System.Net;
using WebShield.Core.Checks;
using WebShield.Core.Models;

namespace WebShield.Core.Tests;

public sealed class CachePolicyCheckTests
{
    [Fact]
    public async Task RunAsync_WhenCacheControlIsMissing_ShouldReturnLowFinding()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("OK")
        };

        using var httpClient = new HttpClient();
        var check = new CachePolicyCheck();

        var findings = await check.RunAsync(new Uri("https://example.com"), httpClient, response);

        Assert.Contains(findings, finding =>
            finding.Title == "Missing Cache-Control header" &&
            finding.Severity == Severity.Low);
    }

    [Fact]
    public async Task RunAsync_WhenPublicCacheSetsCookie_ShouldReturnMediumFinding()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("OK")
        };

        response.Headers.TryAddWithoutValidation("Cache-Control", "public, max-age=3600");
        response.Headers.TryAddWithoutValidation("Set-Cookie", "session=test; Path=/");

        using var httpClient = new HttpClient();
        var check = new CachePolicyCheck();

        var findings = await check.RunAsync(new Uri("https://example.com"), httpClient, response);

        Assert.Contains(findings, finding =>
            finding.Title == "Cookie-setting response may be publicly cacheable" &&
            finding.Severity == Severity.Medium);
    }
}
