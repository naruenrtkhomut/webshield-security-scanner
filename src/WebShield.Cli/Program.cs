using WebShield.Core.Models;
using WebShield.Core.Scanning;
using WebShield.Reporting;

var exitCode = await RunAsync(args);
return exitCode;

static async Task<int> RunAsync(string[] args)
{
    if (args.Length == 0 || args[0] is "-h" or "--help")
    {
        PrintUsage();
        return 0;
    }

    if (!args[0].Equals("scan", StringComparison.OrdinalIgnoreCase))
    {
        Console.Error.WriteLine($"Unknown command: {args[0]}");
        PrintUsage();
        return 1;
    }

    if (args.Length < 2)
    {
        Console.Error.WriteLine("Missing target URL.");
        PrintUsage();
        return 1;
    }

    if (!Uri.TryCreate(args[1], UriKind.Absolute, out var target))
    {
        Console.Error.WriteLine("Invalid target URL. Example: https://example.com");
        return 1;
    }

    var reportPath = GetOptionValue(args, "--report");
    var timeoutSeconds = int.TryParse(GetOptionValue(args, "--timeout"), out var parsedTimeout)
        ? parsedTimeout
        : 15;

    using var httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(timeoutSeconds)
    };

    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("WebShieldSecurityScanner/0.1");

    Console.WriteLine("WebShield Security Scanner");
    Console.WriteLine($"Target: {target}");
    Console.WriteLine();

    var scanner = new WebScanner(httpClient);
    var result = await scanner.ScanAsync(target);

    PrintResult(result);

    if (!string.IsNullOrWhiteSpace(reportPath))
    {
        var directory = Path.GetDirectoryName(reportPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllTextAsync(reportPath, MarkdownReportWriter.Write(result));
        Console.WriteLine();
        Console.WriteLine($"Report written to {reportPath}");
    }

    return result.Findings.Any(f => f.Severity >= Severity.High) ? 2 : 0;
}

static string? GetOptionValue(string[] args, string optionName)
{
    for (var index = 0; index < args.Length - 1; index++)
    {
        if (args[index].Equals(optionName, StringComparison.OrdinalIgnoreCase))
        {
            return args[index + 1];
        }
    }

    return null;
}

static void PrintResult(ScanResult result)
{
    Console.WriteLine($"Score: {result.Score}/100");
    Console.WriteLine();

    foreach (var finding in result.Findings.OrderByDescending(f => f.Severity).ThenBy(f => f.Title))
    {
        Console.WriteLine($"[{finding.Severity}] {finding.Title}");
        Console.WriteLine($"  {finding.Recommendation}");
        Console.WriteLine();
    }
}

static void PrintUsage()
{
    Console.WriteLine("WebShield Security Scanner");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  webshield scan <url> [--report <path>] [--timeout <seconds>]");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  dotnet run --project src/WebShield.Cli -- scan https://example.com");
    Console.WriteLine("  dotnet run --project src/WebShield.Cli -- scan https://example.com --report reports/example.md");
}
