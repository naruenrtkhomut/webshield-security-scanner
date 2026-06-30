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
    var jsonPath = GetOptionValue(args, "--json");
    var noFail = HasOption(args, "--no-fail");
    var failOnOption = GetOptionValue(args, "--fail-on") ?? Severity.High.ToString();

    if (!Enum.TryParse<Severity>(failOnOption, ignoreCase: true, out var failOnSeverity))
    {
        Console.Error.WriteLine($"Invalid severity for --fail-on: {failOnOption}");
        Console.Error.WriteLine("Allowed values: Info, Low, Medium, High, Critical");
        return 1;
    }

    var timeoutSeconds = int.TryParse(GetOptionValue(args, "--timeout"), out var parsedTimeout)
        ? parsedTimeout
        : 15;

    using var httpClient = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(timeoutSeconds)
    };

    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("WebShieldSecurityScanner/0.2");

    Console.WriteLine("WebShield Security Scanner");
    Console.WriteLine($"Target: {target}");
    Console.WriteLine($"Fail-on severity: {(noFail ? "disabled" : failOnSeverity)}");
    Console.WriteLine();

    var scanner = new WebScanner(httpClient);
    var result = await scanner.ScanAsync(target);

    PrintResult(result);

    if (!string.IsNullOrWhiteSpace(reportPath))
    {
        await WriteFileAsync(reportPath, MarkdownReportWriter.Write(result));
        Console.WriteLine();
        Console.WriteLine($"Markdown report written to {reportPath}");
    }

    if (!string.IsNullOrWhiteSpace(jsonPath))
    {
        await WriteFileAsync(jsonPath, JsonReportWriter.Write(result));
        Console.WriteLine();
        Console.WriteLine($"JSON report written to {jsonPath}");
    }

    if (noFail)
    {
        return 0;
    }

    return result.Findings.Any(f => f.Severity >= failOnSeverity) ? 2 : 0;
}

static async Task WriteFileAsync(string path, string content)
{
    var directory = Path.GetDirectoryName(path);
    if (!string.IsNullOrWhiteSpace(directory))
    {
        Directory.CreateDirectory(directory);
    }

    await File.WriteAllTextAsync(path, content);
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

static bool HasOption(string[] args, string optionName)
{
    return args.Any(arg => arg.Equals(optionName, StringComparison.OrdinalIgnoreCase));
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
    Console.WriteLine("  webshield scan <url> [--report <path>] [--json <path>] [--fail-on <severity>] [--no-fail] [--timeout <seconds>]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  --report <path>       Write a Markdown report.");
    Console.WriteLine("  --json <path>         Write a JSON report for automation.");
    Console.WriteLine("  --fail-on <severity>  Return exit code 2 when findings at or above severity exist. Default: High.");
    Console.WriteLine("  --no-fail             Always return exit code 0 after a completed scan.");
    Console.WriteLine("  --timeout <seconds>   HTTP timeout. Default: 15.");
    Console.WriteLine();
    Console.WriteLine("Severity values:");
    Console.WriteLine("  Info, Low, Medium, High, Critical");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  dotnet run --project src/WebShield.Cli -- scan https://example.com");
    Console.WriteLine("  dotnet run --project src/WebShield.Cli -- scan https://example.com --report reports/example.md");
    Console.WriteLine("  dotnet run --project src/WebShield.Cli -- scan https://example.com --json reports/example.json --fail-on Medium");
}
