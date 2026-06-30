using WebShield.Core.Models;

namespace WebShield.Core.Checks;

public sealed class SecurityHeadersCheck : IWebSecurityCheck
{
    public string CheckId => "security-headers";

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
                "Unable to inspect security headers",
                Severity.Info,
                "The target homepage response could not be loaded, so header checks were skipped.",
                "Header hardening status is unknown.",
                "Verify the target URL is reachable and rerun the scan.",
                "No homepage response."));

            return Task.FromResult<IReadOnlyCollection<Finding>>(findings);
        }

        AddIfMissing(homeResponse, findings, "Content-Security-Policy", Severity.High,
            "Missing Content-Security-Policy",
            "A Content Security Policy helps reduce the impact of cross-site scripting and content injection.",
            "Add a strict Content-Security-Policy header and avoid unsafe-inline where possible.");

        if (target.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            AddIfMissing(homeResponse, findings, "Strict-Transport-Security", Severity.Medium,
                "Missing Strict-Transport-Security",
                "HSTS tells browsers to use HTTPS for future requests and helps reduce downgrade risk.",
                "Add Strict-Transport-Security with an appropriate max-age after confirming HTTPS is correctly configured.");
        }

        AddIfMissing(homeResponse, findings, "X-Frame-Options", Severity.Medium,
            "Missing X-Frame-Options",
            "Without frame protection, pages may be more exposed to clickjacking.",
            "Add X-Frame-Options: DENY or SAMEORIGIN, or enforce frame-ancestors in Content-Security-Policy.");

        AddIfMissing(homeResponse, findings, "X-Content-Type-Options", Severity.Low,
            "Missing X-Content-Type-Options",
            "Browsers may try to MIME-sniff responses, which can increase exposure in some content injection scenarios.",
            "Add X-Content-Type-Options: nosniff.");

        AddIfMissing(homeResponse, findings, "Referrer-Policy", Severity.Low,
            "Missing Referrer-Policy",
            "Sensitive URLs may leak through the Referer header when users navigate away from the site.",
            "Add a Referrer-Policy such as strict-origin-when-cross-origin or no-referrer.");

        AddIfMissing(homeResponse, findings, "Permissions-Policy", Severity.Low,
            "Missing Permissions-Policy",
            "Browser features are not explicitly restricted.",
            "Add a Permissions-Policy header that disables features your app does not need.");

        if (findings.Count == 0)
        {
            findings.Add(new Finding(
                CheckId,
                "Security headers look good",
                Severity.Info,
                "The baseline HTTP security headers were present on the homepage response.",
                "No immediate header hardening issue was detected by this check.",
                "Review header values manually to confirm they are strict enough for your application.",
                "Baseline headers present."));
        }

        return Task.FromResult<IReadOnlyCollection<Finding>>(findings);
    }

    private static void AddIfMissing(
        HttpResponseMessage response,
        List<Finding> findings,
        string headerName,
        Severity severity,
        string title,
        string description,
        string recommendation)
    {
        var contentHasHeader = response.Content?.Headers.Contains(headerName) ?? false;
        if (response.Headers.Contains(headerName) || contentHasHeader)
        {
            return;
        }

        findings.Add(new Finding(
            "security-headers",
            title,
            severity,
            description,
            description,
            recommendation,
            $"Header not found: {headerName}"));
    }
}
