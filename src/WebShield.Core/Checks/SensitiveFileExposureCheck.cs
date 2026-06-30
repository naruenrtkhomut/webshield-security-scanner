using System.Net;
using WebShield.Core.Models;

namespace WebShield.Core.Checks;

public sealed class SensitiveFileExposureCheck : IWebSecurityCheck
{
    private static readonly string[] SensitivePaths =
    [
        "/.env",
        "/.git/config",
        "/backup.zip",
        "/database.sql"
    ];

    public string CheckId => "sensitive-file-exposure";

    public async Task<IReadOnlyCollection<Finding>> RunAsync(
        Uri target,
        HttpClient httpClient,
        HttpResponseMessage? homeResponse,
        CancellationToken cancellationToken = default)
    {
        var findings = new List<Finding>();
        var origin = target.GetLeftPart(UriPartial.Authority);

        foreach (var path in SensitivePaths)
        {
            var endpoint = new Uri(origin + path);

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                if (response.StatusCode is HttpStatusCode.OK)
                {
                    findings.Add(new Finding(
                        CheckId,
                        "Potential sensitive file exposure",
                        Severity.High,
                        "A common sensitive file path returned HTTP 200.",
                        "Sensitive files may expose credentials, source control metadata, backups, or database content.",
                        "Block access to sensitive file patterns at the web server, CDN, and application routing layer.",
                        $"Reachable endpoint: {endpoint}"));
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch
            {
                // Keep the check safe and non-fatal.
            }
        }

        if (findings.Count == 0)
        {
            findings.Add(new Finding(
                CheckId,
                "Common sensitive file paths were not publicly reachable",
                Severity.Info,
                "The scanner did not receive HTTP 200 from the baseline sensitive file paths.",
                "No common sensitive file exposure was detected by this baseline check.",
                "Keep deny rules for sensitive file extensions and paths in every environment.",
                "Checked baseline sensitive paths."));
        }

        return findings;
    }
}
