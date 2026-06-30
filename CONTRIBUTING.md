# Contributing

Thank you for considering a contribution to WebShield Security Scanner.

WebShield is a defensive security project. Contributions should help developers audit and harden systems they own or are authorized to test.

## Contribution Scope

Good contributions include:

- Safe security checks
- Better findings and remediation text
- Report generation improvements
- Unit tests
- Documentation
- CI/CD examples
- ASP.NET Core, Angular, Nginx, Docker, and Kubernetes hardening guides
- Bug fixes and refactoring

Out-of-scope contributions include:

- Auto exploitation
- Credential attacks
- Brute force functionality
- WAF bypass or evasion
- Stealth scanning
- Persistence or post-exploitation logic
- Mass internet scanning features
- Destructive payloads

## Development Setup

```bash
git clone https://github.com/naruenrtkhomut/webshield-security-scanner.git
cd webshield-security-scanner

dotnet restore
dotnet build
dotnet test
```

Run the CLI:

```bash
dotnet run --project src/WebShield.Cli -- scan https://example.com
```

Generate a report:

```bash
dotnet run --project src/WebShield.Cli -- scan https://example.com --report reports/example.md
```

## Coding Guidelines

- Keep checks safe and non-destructive.
- Prefer clear remediation messages over noisy findings.
- Add unit tests for scoring, checks, and report generation.
- Keep network requests minimal and transparent.
- Do not add aggressive crawling by default.
- Use nullable reference types correctly.
- Keep public APIs small and explicit.

## Pull Request Checklist

Before opening a pull request:

- [ ] The change matches the defensive security scope.
- [ ] The project builds locally.
- [ ] Tests pass locally.
- [ ] New behavior is documented.
- [ ] Findings include clear impact and recommendation text.
- [ ] No exploit, brute-force, evasion, or destructive logic was added.

## Security Issues

Do not open public issues for sensitive vulnerabilities in WebShield itself. See [SECURITY.md](SECURITY.md).
