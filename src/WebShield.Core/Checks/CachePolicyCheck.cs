using WebShield.Core.Models;

namespace WebShield.Core.Checks;

public sealed class CachePolicyCheck : IWebSecurityCheck
{
    public string CheckId => "cache-policy";

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
                "Unable to inspect cache policy",
                Severity.Info,
                "The target homepage response could not be loaded, so cache policy checks were skipped.",
                "Cache behavior status is unknown.",
                "Verify the target URL is reachable and rerun the scan.",
                "No homepage response."));

            return Task.FromResult<IReadOnlyCollection<Finding>>(findings);
        }

        var cacheControlValues = GetHeaderValues(homeResponse, "Cache-Control");
        var pragmaValues = GetHeaderValues(homeResponse, "Pragma");
        var hasSetCookie = homeResponse.Headers.Contains("Set-Cookie");

        if (cacheControlValues.Count == 0)
        {
            findings.Add(new Finding(
                CheckId,
                "Missing Cache-Control header",
                Severity.Low,
                "The baseline response does not include a Cache-Control header.",
                "Without explicit cache policy, browsers, proxies, or CDNs may cache responses in ways that are not intended.",
                "Add an explicit Cache-Control policy. Use no-store for sensitive pages and appropriate public caching for static assets.",
                $"Cache-Control not found; Pragma: {FormatValues(pragmaValues)}"));

            return Task.FromResult<IReadOnlyCollection<Finding>>(findings);
        }

        var cacheControl = string.Join(", ", cacheControlValues);
        var isPublicCache = cacheControlValues.Any(value => value.Contains("public", StringComparison.OrdinalIgnoreCase));
        var hasPrivateDirective = cacheControlValues.Any(value => value.Contains("private", StringComparison.OrdinalIgnoreCase));
        var hasNoStoreDirective = cacheControlValues.Any(value => value.Contains("no-store", StringComparison.OrdinalIgnoreCase));

        if (hasSetCookie && isPublicCache && !hasPrivateDirective && !hasNoStoreDirective)
        {
            findings.Add(new Finding(
                CheckId,
                "Cookie-setting response may be publicly cacheable",
                Severity.Medium,
                "The response sets cookies and also appears to allow public caching.",
                "Publicly cacheable responses that set cookies can lead to user-specific data being cached or shared incorrectly depending on infrastructure behavior.",
                "Avoid public caching for responses that set cookies. Prefer Cache-Control: private or no-store for user-specific responses.",
                $"Cache-Control: {cacheControl}; Set-Cookie observed: yes"));
        }
        else
        {
            findings.Add(new Finding(
                CheckId,
                "Cache-Control header observed",
                Severity.Info,
                "The baseline response includes an explicit Cache-Control policy.",
                "Explicit caching policy reduces ambiguity for browsers, proxies, and CDNs.",
                "Review whether the chosen directive is appropriate for static, public, authenticated, and sensitive routes.",
                $"Cache-Control: {cacheControl}; Set-Cookie observed: {(hasSetCookie ? "yes" : "no")}"));
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

    private static string FormatValues(IReadOnlyCollection<string> values)
    {
        return values.Count == 0 ? "not found" : string.Join(", ", values);
    }
}
