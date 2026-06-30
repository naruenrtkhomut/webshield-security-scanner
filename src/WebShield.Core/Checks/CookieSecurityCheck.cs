using WebShield.Core.Models;

namespace WebShield.Core.Checks;

public sealed class CookieSecurityCheck : IWebSecurityCheck
{
    public string CheckId => "cookie-security";

    public Task<IReadOnlyCollection<Finding>> RunAsync(
        Uri target,
        HttpClient httpClient,
        HttpResponseMessage? homeResponse,
        CancellationToken cancellationToken = default)
    {
        var findings = new List<Finding>();

        if (homeResponse is null || !homeResponse.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            findings.Add(new Finding(
                CheckId,
                "No cookies observed on homepage response",
                Severity.Info,
                "The scanner did not observe Set-Cookie headers on the initial response.",
                "Cookie flag status is unknown for authenticated areas.",
                "Run an authenticated scan profile in a future version to inspect application session cookies.",
                "No Set-Cookie headers found."));

            return Task.FromResult<IReadOnlyCollection<Finding>>(findings);
        }

        foreach (var cookie in cookies)
        {
            var cookieName = cookie.Split('=', 2)[0].Trim();

            if (target.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) &&
                !cookie.Contains("Secure", StringComparison.OrdinalIgnoreCase))
            {
                findings.Add(new Finding(
                    CheckId,
                    $"Cookie '{cookieName}' is missing Secure flag",
                    Severity.Medium,
                    "Secure cookies are only sent over HTTPS.",
                    "Without Secure, sensitive cookies may be sent over an insecure connection if HTTP is reachable.",
                    "Add the Secure flag to sensitive cookies.",
                    cookie));
            }

            if (!cookie.Contains("HttpOnly", StringComparison.OrdinalIgnoreCase))
            {
                findings.Add(new Finding(
                    CheckId,
                    $"Cookie '{cookieName}' is missing HttpOnly flag",
                    Severity.Medium,
                    "HttpOnly helps prevent client-side scripts from reading cookies.",
                    "If an XSS issue exists, cookies without HttpOnly are easier to steal.",
                    "Add the HttpOnly flag to session and authentication cookies.",
                    cookie));
            }

            if (!cookie.Contains("SameSite", StringComparison.OrdinalIgnoreCase))
            {
                findings.Add(new Finding(
                    CheckId,
                    $"Cookie '{cookieName}' is missing SameSite attribute",
                    Severity.Low,
                    "SameSite helps reduce cross-site request forgery risk.",
                    "Cookies without SameSite may be sent in more cross-site request contexts.",
                    "Set SameSite=Lax or SameSite=Strict where compatible with your application flow.",
                    cookie));
            }
        }

        if (findings.Count == 0)
        {
            findings.Add(new Finding(
                CheckId,
                "Observed cookies include baseline security flags",
                Severity.Info,
                "The cookies observed on the homepage response included Secure, HttpOnly, and SameSite where expected.",
                "No immediate cookie flag issue was detected by this check.",
                "Review authenticated cookies manually in security-sensitive flows.",
                "Baseline cookie flags present."));
        }

        return Task.FromResult<IReadOnlyCollection<Finding>>(findings);
    }
}
