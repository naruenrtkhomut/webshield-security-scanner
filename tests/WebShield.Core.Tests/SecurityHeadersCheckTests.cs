using System.Net;
using WebShield.Core.Checks;

namespace WebShield.Core.Tests;

public sealed class SecurityHeadersCheckTests
{
    [Fact]
    public async Task RunAsync_WhenCspIsMissing_ShouldReturnFinding()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("OK")
        };

        using var httpClient = new HttpClient();
        var check = new SecurityHeadersCheck();

        var findings = await check.RunAsync(new Uri("https://example.com"), httpClient, response);

        Assert.Contains(findings, finding => finding.Title == "Missing Content-Security-Policy");
    }
}
