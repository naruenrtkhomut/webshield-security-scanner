using WebShield.Core.Models;

namespace WebShield.Core.Checks;

public sealed class InformationDisclosureHeadersCheck : IWebSecurityCheck
{
    private static readonly string[] DisclosureHeaders =
    [
        "Server",
        "X-Powered-By",
        "X-AspNet-Version",
        "X-AspNetMvc-Version",
        "X-Generator"
    ];

    public string CheckId => "information-disclosure-headers";

    public Task<IReadOnlyCollection<Finding>> RunAsync(
        Uri target,
        HttpClient httpClient,
        HttpResponseMessage? homeResponse,
        CancellationToken cancellationToken = default)
    {
        var findings = new List<Finding>();

        if (homeResponse is null)
        {
            findings.Add(new Finding(
                CheckId,
                "Unable to inspect information disclosure headers",
                Severity.Info,
                "The target homepage response could not be loaded, so information disclosure header checks were skipped.",
                "Technology disclosure status is unknown.",
                "Verify the target URL is reachable and rerun the scan.",
                "No homepage response."));

            return Task.FromResult<IReadOnlyCollection<Finding>>(findings);
        }

        foreach (var headerName in DisclosureHeaders)
        {
            var values = GetHeaderValues(homeResponse, headerName);
            if (values.Count == 0)
            {
                continue;
            }

            findings.Add(new Finding(
                CheckId,
                $"Technology disclosure header observed: {headerName}",
                Severity.Low,
                "The response exposes a header that may reveal server, framework, or generator information.",
                "Technology disclosure can help attackers tailor reconnaissance and version-specific attacks, especially when exact versions are exposed.",
                "Remove or minimize technology-identifying headers at the application, reverse proxy, CDN, or web server layer.",
                $"{headerName}: {string.Join(", ", values)}"));
        }

        if (findings.Count == 0)
        {
            findings.Add(new Finding(
                CheckId,
                "No common technology disclosure headers observed",
                Severity.Info,
                "The baseline response did not include common server or framework disclosure headers checked by WebShield.",
                "No immediate technology disclosure issue was detected by this baseline check.",
                "Continue minimizing framework and platform details in production responses.",
                "Checked common disclosure headers."));
        }

        return Task.FromResult<IReadOnlyCollection<Finding>>(findings);
    }

    private static IReadOnlyCollection<string> GetHeaderValues(HttpResponseMessage response, string headerName)
    {
        if (response.Headers.TryGetValues(headerName, out var responseValues))
        {
            return responseValues.ToArray();
        }

        if (response.Content.Headers.TryGetValues(headerName, out var contentValues))
        {
            return contentValues.ToArray();
        }

        return [];
    }
}
