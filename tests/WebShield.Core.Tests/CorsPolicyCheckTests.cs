using System.Net;
using WebShield.Core.Checks;
using WebShield.Core.Models;

namespace WebShield.Core.Tests;

public sealed class CorsPolicyCheckTests
{
    [Fact]
    public async Task RunAsync_WhenWildcardOriginAllowsCredentials_ShouldReturnHighFinding()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("OK")
        };

        response.Headers.TryAddWithoutValidation("Access-Control-Allow-Origin", "*");
        response.Headers.TryAddWithoutValidation("Access-Control-Allow-Credentials", "true");

        using var httpClient = new HttpClient();
        var check = new CorsPolicyCheck();

        var findings = await check.RunAsync(new Uri("https://example.com"), httpClient, response);

        Assert.Contains(findings, finding =>
            finding.Title == "Wildcard CORS origin allows credentials" &&
            finding.Severity == Severity.High);
    }

    [Fact]
    public async Task RunAsync_WhenNoCorsHeadersObserved_ShouldReturnInfoFinding()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("OK")
        };

        using var httpClient = new HttpClient();
        var check = new CorsPolicyCheck();

        var findings = await check.RunAsync(new Uri("https://example.com"), httpClient, response);

        Assert.Contains(findings, finding =>
            finding.Title == "No CORS headers observed on homepage response" &&
            finding.Severity == Severity.Info);
    }
}
