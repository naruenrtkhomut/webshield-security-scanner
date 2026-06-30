using WebShield.Core.Models;

namespace WebShield.Core.Checks;

public sealed class TransportSecurityCheck : IWebSecurityCheck
{
    public string CheckId => "transport-security";

    public Task<IReadOnlyCollection<Finding>> RunAsync(
        Uri target,
        HttpClient httpClient,
        HttpResponseMessage? homeResponse,
        CancellationToken cancellationToken = default)
    {
        var findings = new List<Finding>();
        var finalUri = homeResponse?.RequestMessage?.RequestUri;

        if (target.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            findings.Add(new Finding(
                CheckId,
                "HTTPS is enabled for the target URL",
                Severity.Info,
                "The target URL uses HTTPS.",
                "HTTPS protects traffic confidentiality and integrity between the browser and server.",
                "Keep HTTPS enabled for every environment and combine it with HSTS after validating the deployment.",
                $"Initial target: {target}; final URL: {finalUri?.ToString() ?? "unknown"}"));

            return Task.FromResult<IReadOnlyCollection<Finding>>(findings);
        }

        if (target.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
        {
            if (finalUri is not null && finalUri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                findings.Add(new Finding(
                    CheckId,
                    "HTTP redirects to HTTPS",
                    Severity.Info,
                    "The target was requested over HTTP and the final response URL uses HTTPS.",
                    "Redirecting HTTP to HTTPS reduces accidental insecure access.",
                    "Keep the redirect in place and verify that all public HTTP routes redirect consistently.",
                    $"Initial target: {target}; final URL: {finalUri}"));
            }
            else
            {
                findings.Add(new Finding(
                    CheckId,
                    "Target does not use HTTPS",
                    Severity.Medium,
                    "The target URL uses HTTP and did not appear to redirect to HTTPS during the baseline request.",
                    "Traffic may be exposed to interception or modification when a site is available over plain HTTP.",
                    "Serve the site over HTTPS and redirect all HTTP requests to HTTPS before production release.",
                    $"Initial target: {target}; final URL: {finalUri?.ToString() ?? "unknown"}"));
            }
        }

        return Task.FromResult<IReadOnlyCollection<Finding>>(findings);
    }
}
