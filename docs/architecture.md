# Architecture

WebShield is split into three main layers.

```text
CLI
 │
 ├── parses commands and options
 ├── validates target URL
 └── writes console output

Core
 │
 ├── owns scanner orchestration
 ├── executes safe security checks
 ├── normalizes findings
 └── calculates risk score

Reporting
 │
 ├── converts scan result into Markdown/HTML/PDF later
 └── provides product-ready report templates
```

## Projects

| Project | Purpose |
|---|---|
| `WebShield.Cli` | Command-line entry point |
| `WebShield.Core` | Scanner engine, checks, findings, scoring |
| `WebShield.Reporting` | Report rendering and export |
| `WebShield.Core.Tests` | Unit tests for scanner logic |

## Scan Flow

```text
User runs command
  ↓
CLI validates target and options
  ↓
WebScanner sends safe HTTP request
  ↓
Checks inspect headers, cookies, common metadata, and known safe endpoints
  ↓
Findings are grouped and scored
  ↓
Report writer exports result
```

## Safety Model

The scanner must avoid destructive behavior. Checks should be passive or minimally active:

- Prefer `GET` and `HEAD` requests.
- Do not submit forms.
- Do not attempt login bypass.
- Do not brute force credentials or paths.
- Do not run exploit payloads.
- Do not perform high-volume crawling by default.

## Future Components

- HTML/PDF report renderer
- CI/CD quality gate mode
- OpenAPI parser
- Authenticated scan profile for owned systems
- Web dashboard/SaaS version
- Plugin system for safe checks
