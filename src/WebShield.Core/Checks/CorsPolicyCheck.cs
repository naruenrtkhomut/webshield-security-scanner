using WebShield.Core.Models;

namespace WebShield.Core.Checks;

public sealed class CorsPolicyCheck : IWebSecurityCheck
{
    public string CheckId => "cors-policy";

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
                "Unable to inspect CORS policy",
                Severity.Info,
                "The target homepage response could not be loaded, so CORS checks were skipped.",
                "CORS exposure status is unknown.",
                "Verify the target URL is reachable and rerun the scan.",
                "No homepage response."));

            return Task.FromResult<IReadOnlyCollection<Finding>>(findings);
        }

        var allowOrigin = GetHeaderValues(homeResponse, "Access-Control-Allow-Origin");
        var allowCredentials = GetHeaderValues(homeResponse, "Access-Control-Allow-Credentials");

        if (allowOrigin.Count == 0 && allowCredentials.Count == 0)
        {
            findings.Add(new Finding(
                CheckId,
                "No CORS headers observed on homepage response",
                Severity.Info,
                "The scanner did not observe Access-Control-Allow-Origin or Access-Control-Allow-Credentials on the baseline response.",
                "No CORS exposure was detected by this baseline check.",
                "If this is an API, verify CORS behavior on API endpoints and authenticated flows as part of manual review.",
                "No baseline CORS headers found."));

            return Task.FromResult<IReadOnlyCollection<Finding>>(findings);
        }

        var originEvidence = string.Join(", ", allowOrigin);
        var credentialsEvidence = string.Join(", ", allowCredentials);
        var evidence = $"Access-Control-Allow-Origin: {originEvidence}; Access-Control-Allow-Credentials: {credentialsEvidence}";

        var hasWildcardOrigin = allowOrigin.Any(value => value.Trim() == "*");
        var hasNullOrigin = allowOrigin.Any(value => value.Contains("null", StringComparison.OrdinalIgnoreCase));
        var allowsCredentials = allowCredentials.Any(value => value.Trim().Equals("true", StringComparison.OrdinalIgnoreCase));

        if (hasWildcardOrigin && allowsCredentials)
        {
            findings.Add(new Finding(
                CheckId,
                "Wildcard CORS origin allows credentials",
                Severity.High,
                "The response allows any origin and also indicates that credentials may be included.",
                "Overly permissive CORS policies can expose authenticated browser data to untrusted origins when combined with sensitive endpoints.",
                "Use an explicit allowlist of trusted origins and avoid allowing credentials unless the endpoint truly requires cross-origin authenticated browser requests.",
                evidence));
        }
        else if (hasWildcardOrigin)
        {
            findings.Add(new Finding(
                CheckId,
                "Wildcard CORS origin observed",
                Severity.Low,
                "The response uses Access-Control-Allow-Origin: *.",
                "Wildcard CORS may be acceptable for public static resources, but it is risky for private APIs or user-specific data.",
                "Restrict CORS to trusted origins for APIs and authenticated endpoints.",
                evidence));
        }
        else if (hasNullOrigin)
        {
            findings.Add(new Finding(
                CheckId,
                "CORS policy allows null origin",
                Severity.Medium,
                "The response appears to allow the null origin.",
                "The null origin can appear in sandboxed or local-file contexts and should rarely be trusted for sensitive applications.",
                "Remove null from allowed origins unless there is a documented and reviewed business requirement.",
                evidence));
        }
        else
        {
            findings.Add(new Finding(
                CheckId,
                "CORS headers observed",
                Severity.Info,
                "The response includes CORS headers, but no baseline wildcard or null-origin issue was detected.",
                "CORS behavior may still vary by endpoint, method, and origin.",
                "Review API CORS configuration manually and prefer explicit trusted origin allowlists.",
                evidence));
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
