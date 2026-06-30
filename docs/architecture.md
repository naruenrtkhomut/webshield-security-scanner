# Architecture

WebShield is split into three main layers.

```text
CLI
 │
 ├── parses commands and options
 ├── validates target URL
 ├── applies quality-gate options
 └── writes console output

Core
 │
 ├── owns scanner orchestration
 ├── executes safe security checks
 ├── normalizes findings
 └── calculates risk score

Reporting
 │
 ├── converts scan result into Markdown and JSON
 └── provides product-ready report templates
```

## Projects

| Project | Purpose |
|---|---|
| `WebShield.Cli` | Command-line entry point, report options, and quality-gate exit behavior |
| `WebShield.Core` | Scanner engine, checks, findings, scoring |
| `WebShield.Reporting` | Markdown and JSON report rendering |
| `WebShield.Core.Tests` | Unit tests for scanner logic and reporting |

## Default Check Pipeline

The default scanner pipeline is intentionally safe and low-volume:

1. `TransportSecurityCheck`
2. `SecurityHeadersCheck`
3. `CookieSecurityCheck`
4. `CorsPolicyCheck`
5. `InformationDisclosureHeadersCheck`
6. `CachePolicyCheck`
7. `SwaggerExposureCheck`
8. `SensitiveFileExposureCheck`

## Scan Flow

```text
User runs command
  ↓
CLI validates target and options
  ↓
WebScanner sends safe baseline HTTP request
  ↓
Checks inspect transport, headers, cookies, CORS, cache policy, metadata, and known safe endpoints
  ↓
Findings are grouped and scored
  ↓
Markdown and/or JSON report writers export result
  ↓
CLI applies quality-gate exit code when enabled
```

## Reporting Flow

```text
ScanResult
  ├── Target
  ├── StartedAt / FinishedAt
  ├── Score
  └── Findings
        ↓
MarkdownReportWriter → human-readable audit report
JsonReportWriter     → automation and CI/CD output
```

## Quality Gate Flow

By default, the CLI returns exit code `2` when a finding is `High` or `Critical`.

This behavior can be changed:

```bash
--fail-on Medium
```

Or disabled:

```bash
--no-fail
```

## Safety Model

The scanner must avoid destructive behavior. Checks should be passive or minimally active:

- Prefer `GET` and `HEAD` requests.
- Do not submit forms.
- Do not attempt login bypass.
- Do not brute force credentials or paths.
- Do not run exploit payloads.
- Do not perform high-volume crawling by default.
- Do not store response bodies from sensitive-looking files.
- Keep evidence concise and remediation-focused.

## Future Components

- HTML/PDF report renderer
- SARIF output for GitHub code scanning workflows
- OpenAPI parser
- Authenticated scan profile for owned systems
- Web dashboard/SaaS version
- Plugin system for safe checks
