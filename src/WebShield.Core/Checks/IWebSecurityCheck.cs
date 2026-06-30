using WebShield.Core.Models;

namespace WebShield.Core.Checks;

public interface IWebSecurityCheck
{
    string CheckId { get; }

    Task<IReadOnlyCollection<Finding>> RunAsync(
        Uri target,
        HttpClient httpClient,
        HttpResponseMessage? homeResponse,
        CancellationToken cancellationToken = default);
}
