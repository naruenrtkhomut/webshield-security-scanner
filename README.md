# WebShield Security Scanner

Developer-first web security audit and vulnerability reporting tool for websites, APIs, and modern web applications.

> WebShield is designed for defensive security testing on systems you own or are explicitly authorized to test. It is not an exploit framework, brute-force tool, or mass scanning tool.

## Support This Project

If WebShield helps you improve your web application security workflow, consider supporting development through GitHub Sponsors.

Your sponsorship helps fund:

- New defensive security checks
- Better Markdown, HTML, and PDF reports
- CI/CD integration examples
- ASP.NET Core, Angular, Nginx, Docker, and Kubernetes remediation guides
- Documentation, examples, and maintenance

See [docs/sponsorship.md](docs/sponsorship.md) for suggested sponsor tiers and what the funding supports.

## Goals

- Help developers find common web security misconfigurations before production.
- Generate clear reports that explain risk, impact, and remediation.
- Provide practical fix guidance for ASP.NET Core, Angular, Nginx, Docker, and Kubernetes-based apps.
- Fit into local development, CI/CD, and pre-release security checks.

## MVP Scope

The first version focuses on safe, non-destructive checks:

- Transport security and HTTPS redirect checks
- HTTP security headers
- Cookie flags
- CORS policy review
- Information disclosure header detection
- Cache policy hardening checks
- Swagger/OpenAPI public exposure checks
- Common sensitive file exposure checks
- Risk score and severity classification
- Markdown and JSON report export
- CI/CD quality gate with configurable fail severity

## Testing and Quality

WebShield includes a dedicated testing documentation set for contributors, maintainers, and release validation:

- [Security Scan Upgrade Notes](docs/security-scan-upgrades.md) — summary of the expanded defensive scan checks and automation features
- [Testing Strategy](docs/testing-strategy.md) — unit, component, integration, manual QA, regression, and CI quality gates
- [Manual Testing Guide](docs/manual-testing-guide.md) — safe hands-on validation commands and release QA checklist
- [Test Case Matrix](docs/test-case-matrix.md) — detailed expected behavior for scanner core, checks, CLI, reporting, and docs
- [Safe Testing Lab Guide](docs/safe-testing-lab.md) — localhost-first lab design for authorized defensive testing

Run the automated test suite:

```bash
dotnet test
```

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
├── .github/
│   ├── FUNDING.yml           # GitHub Sponsors button configuration
│   └── workflows/            # CI pipeline
├── docs/                     # Architecture, roadmap, testing, security scope
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

Generate Markdown and JSON reports:

```bash
dotnet run --project src/WebShield.Cli -- scan https://example.com --report reports/example.md --json reports/example.json
```

Use WebShield as a CI/CD quality gate:

```bash
dotnet run --project src/WebShield.Cli -- scan https://example.com --json reports/example.json --fail-on Medium
```

Disable quality-gate failure while still generating reports:

```bash
dotnet run --project src/WebShield.Cli -- scan https://example.com --json reports/example.json --no-fail
```

## Example Output

```text
WebShield Security Scanner
Target: https://example.com
Fail-on severity: High

[High] Missing Content-Security-Policy
[Medium] Missing X-Frame-Options
[Low] Wildcard CORS origin observed
[Info] HTTPS is enabled for the target URL

Markdown report written to reports/example.md
JSON report written to reports/example.json
```

## Suggested Product Positioning

**WebShield Security Scanner**  
A developer-first web security audit and vulnerability reporting tool for websites, APIs, and modern web applications.

## Contributing

Contributions are welcome when they align with the project's defensive security scope. See [CONTRIBUTING.md](CONTRIBUTING.md).

## License

MIT License. See [LICENSE](LICENSE).

## Responsible Use

Use this tool only against systems that you own or have explicit authorization to test. See [SECURITY.md](SECURITY.md) and [docs/legal-and-ethical-use.md](docs/legal-and-ethical-use.md).
