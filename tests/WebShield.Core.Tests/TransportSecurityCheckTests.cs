using System.Net;
using WebShield.Core.Checks;
using WebShield.Core.Models;

namespace WebShield.Core.Tests;

public sealed class TransportSecurityCheckTests
{
    [Fact]
    public async Task RunAsync_WhenTargetUsesHttps_ShouldReturnInfoFinding()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            RequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://example.com")
        };

        using var httpClient = new HttpClient();
        var check = new TransportSecurityCheck();

        var findings = await check.RunAsync(new Uri("https://example.com"), httpClient, response);

        Assert.Contains(findings, finding =>
            finding.Title == "HTTPS is enabled for the target URL" &&
            finding.Severity == Severity.Info);
    }

    [Fact]
    public async Task RunAsync_WhenHttpTargetDoesNotRedirectToHttps_ShouldReturnMediumFinding()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            RequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://example.com")
        };

        using var httpClient = new HttpClient();
        var check = new TransportSecurityCheck();

        var findings = await check.RunAsync(new Uri("http://example.com"), httpClient, response);

        Assert.Contains(findings, finding =>
            finding.Title == "Target does not use HTTPS" &&
            finding.Severity == Severity.Medium);
    }

    [Fact]
    public async Task RunAsync_WhenHttpTargetRedirectsToHttps_ShouldReturnInfoFinding()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            RequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://example.com")
        };

        using var httpClient = new HttpClient();
        var check = new TransportSecurityCheck();

        var findings = await check.RunAsync(new Uri("http://example.com"), httpClient, response);

        Assert.Contains(findings, finding =>
            finding.Title == "HTTP redirects to HTTPS" &&
            finding.Severity == Severity.Info);
    }
}
