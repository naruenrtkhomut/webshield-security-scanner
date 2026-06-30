# WebShield Security Scanner

Developer-first web security audit and vulnerability reporting tool for websites, APIs, and modern web applications.

> WebShield is designed for defensive security testing on systems you own or are explicitly authorized to test. It is not an exploit framework, brute-force tool, or mass scanning tool.

## Goals

- Help developers find common web security misconfigurations before production.
- Generate clear reports that explain risk, impact, and remediation.
- Provide practical fix guidance for ASP.NET Core, Angular, Nginx, Docker, and Kubernetes-based apps.
- Fit into local development, CI/CD, and pre-release security checks.

## MVP Scope

The first version focuses on safe, non-destructive checks:

- HTTP security headers
- Cookie flags
- HTTPS/TLS availability summary
- Swagger/OpenAPI public exposure checks
- Common sensitive file exposure checks
- Risk score and severity classification
- Markdown report export

## Out of Scope

The public version intentionally excludes:

- Auto exploitation
- Credential attacks
- Brute force testing
- WAF bypass/evasion
- Destructive payloads
- Mass internet scanning

## Repository Structure

```text
webshield-security-scanner/
├── .github/workflows/        # CI pipeline
├── docs/                     # Architecture, roadmap, security scope
├── src/
│   ├── WebShield.Cli/        # CLI entry point
│   ├── WebShield.Core/       # Scanner engine and checks
│   └── WebShield.Reporting/  # Report generation
├── tests/
│   └── WebShield.Core.Tests/ # Unit tests
├── .gitignore
├── Directory.Build.props
├── LICENSE
└── README.md
```

## Quick Start

```bash
dotnet restore
dotnet build

dotnet run --project src/WebShield.Cli -- scan https://example.com
```

Generate a Markdown report:

```bash
dotnet run --project src/WebShield.Cli -- scan https://example.com --report reports/example.md
```

## Example Output

```text
WebShield Security Scanner
Target: https://example.com

[Medium] Missing Content-Security-Policy
[Low] Missing X-Frame-Options
[Info] HTTPS is enabled

Report written to reports/example.md
```

## Suggested Product Positioning

**WebShield Security Scanner**  
A developer-first web security audit and vulnerability reporting tool for websites, APIs, and modern web applications.

## License

MIT License. See [LICENSE](LICENSE).

## Responsible Use

Use this tool only against systems that you own or have explicit authorization to test. See [SECURITY.md](SECURITY.md) and [docs/legal-and-ethical-use.md](docs/legal-and-ethical-use.md).
