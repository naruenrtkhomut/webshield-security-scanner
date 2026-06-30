using System.Net;
using WebShield.Core.Models;

namespace WebShield.Core.Checks;

public sealed class SwaggerExposureCheck : IWebSecurityCheck
{
    private static readonly string[] SwaggerPaths =
    [
        "/swagger",
        "/swagger/index.html",
        "/swagger/v1/swagger.json",
        "/openapi.json"
    ];

    public string CheckId => "swagger-exposure";

    public async Task<IReadOnlyCollection<Finding>> RunAsync(
        Uri target,
        HttpClient httpClient,
        HttpResponseMessage? homeResponse,
        CancellationToken cancellationToken = default)
    {
        var findings = new List<Finding>();

        foreach (var path in SwaggerPaths)
        {
            var endpoint = new Uri(target.GetLeftPart(UriPartial.Authority) + path);

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                if (response.StatusCode is HttpStatusCode.OK)
                {
                    findings.Add(new Finding(
                        CheckId,
                        "Public API documentation may be exposed",
                        Severity.Medium,
                        "A common Swagger/OpenAPI endpoint returned HTTP 200.",
                        "Public API documentation can reveal routes, parameters, models, and internal implementation details.",
                        "Restrict API documentation in production or require authentication for internal APIs.",
                        $"Reachable endpoint: {endpoint}"));
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch
            {
                // Keep the check non-fatal. Network errors for optional endpoints should not stop the scan.
            }
        }

        if (findings.Count == 0)
        {
            findings.Add(new Finding(
                CheckId,
                "Common Swagger/OpenAPI endpoints were not publicly reachable",
                Severity.Info,
                "The scanner did not receive HTTP 200 from common API documentation endpoints.",
                "No public API documentation exposure was detected by this baseline check.",
                "If API documentation is required, keep it authenticated or limited to non-production environments.",
                "Checked common Swagger/OpenAPI paths."));
        }

        return findings;
    }
}
