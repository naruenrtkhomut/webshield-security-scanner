using WebShield.Core.Checks;
using WebShield.Core.Models;

namespace WebShield.Core.Scanning;

public sealed class WebScanner
{
    private readonly HttpClient _httpClient;
    private readonly IReadOnlyCollection<IWebSecurityCheck> _checks;

    public WebScanner(HttpClient httpClient, IReadOnlyCollection<IWebSecurityCheck>? checks = null)
    {
        _httpClient = httpClient;
        _checks = checks ??
        [
            new TransportSecurityCheck(),
            new SecurityHeadersCheck(),
            new CookieSecurityCheck(),
            new SwaggerExposureCheck(),
            new SensitiveFileExposureCheck()
        ];
    }

    public async Task<ScanResult> ScanAsync(Uri target, CancellationToken cancellationToken = default)
    {
        if (target.Scheme is not "http" and not "https")
        {
            throw new ArgumentException("Target URL must use http or https.", nameof(target));
        }

        var result = new ScanResult
        {
            Target = target,
            StartedAt = DateTimeOffset.UtcNow
        };

        HttpResponseMessage? homeResponse = null;

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, target);
            homeResponse = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            result.Findings.Add(new Finding(
                "target-reachability",
                "Target could not be reached",
                Severity.High,
                "The scanner could not load the target URL.",
                "Security checks may be incomplete because the target was unreachable.",
                "Verify DNS, network access, TLS configuration, and target availability.",
                ex.Message));
        }

        foreach (var check in _checks)
        {
            var findings = await check.RunAsync(target, _httpClient, homeResponse, cancellationToken);
            result.Findings.AddRange(findings);
        }

        homeResponse?.Dispose();
        result.FinishedAt = DateTimeOffset.UtcNow;

        return result;
    }
}
