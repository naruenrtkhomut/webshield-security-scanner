using System.Net;
using WebShield.Core.Checks;
using WebShield.Core.Models;

namespace WebShield.Core.Tests;

public sealed class InformationDisclosureHeadersCheckTests
{
    [Fact]
    public async Task RunAsync_WhenServerHeaderIsPresent_ShouldReturnLowFinding()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("OK")
        };

        response.Headers.TryAddWithoutValidation("Server", "test-server/1.0");

        using var httpClient = new HttpClient();
        var check = new InformationDisclosureHeadersCheck();

        var findings = await check.RunAsync(new Uri("https://example.com"), httpClient, response);

        Assert.Contains(findings, finding =>
            finding.Title == "Technology disclosure header observed: Server" &&
            finding.Severity == Severity.Low);
    }

    [Fact]
    public async Task RunAsync_WhenNoDisclosureHeadersArePresent_ShouldReturnInfoFinding()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("OK")
        };

        using var httpClient = new HttpClient();
        var check = new InformationDisclosureHeadersCheck();

        var findings = await check.RunAsync(new Uri("https://example.com"), httpClient, response);

        Assert.Contains(findings, finding =>
            finding.Title == "No common technology disclosure headers observed" &&
            finding.Severity == Severity.Info);
    }
}
