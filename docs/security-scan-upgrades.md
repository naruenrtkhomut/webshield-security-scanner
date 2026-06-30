# Security Scan Upgrade Notes

This document summarizes the expanded defensive scan features added to WebShield Security Scanner.

## Upgrade Summary

The scanner now includes additional safe, baseline checks and automation-friendly output:

- CORS policy review
- Information disclosure header detection
- Cache policy hardening check
- JSON report output
- Configurable CI/CD quality gate
- Non-failing report-only mode

These upgrades keep WebShield focused on defensive security testing for systems you own or are explicitly authorized to test.

## New Defensive Checks

## CORS Policy Review

The `CorsPolicyCheck` inspects baseline CORS headers from the homepage response.

It checks:

- `Access-Control-Allow-Origin`
- `Access-Control-Allow-Credentials`

Findings include:

| Finding | Severity | Why it matters |
|---|---|---|
| Wildcard CORS origin allows credentials | High | Overly permissive cross-origin authenticated browser access can expose sensitive API behavior. |
| Wildcard CORS origin observed | Low | Wildcard CORS may be acceptable for public resources but risky for private APIs. |
| CORS policy allows null origin | Medium | Null origins are rarely appropriate for sensitive applications. |
| No CORS headers observed | Info | No baseline CORS exposure was detected on the homepage response. |
| CORS headers observed | Info | Explicit CORS headers were present without baseline wildcard/null-origin issues. |

## Information Disclosure Header Detection

The `InformationDisclosureHeadersCheck` detects response headers that may reveal server, framework, or generator details.

Checked headers:

- `Server`
- `X-Powered-By`
- `X-AspNet-Version`
- `X-AspNetMvc-Version`
- `X-Generator`

These are Low severity hardening findings because they help reduce technology fingerprinting.

## Cache Policy Hardening

The `CachePolicyCheck` reviews baseline caching behavior.

Findings include:

| Finding | Severity | Why it matters |
|---|---|---|
| Missing Cache-Control header | Low | Without explicit cache policy, browsers, proxies, and CDNs may cache unexpectedly. |
| Cookie-setting response may be publicly cacheable | Medium | Responses that set cookies should not usually be publicly cacheable. |
| Cache-Control header observed | Info | Explicit caching policy is present and should be reviewed by route type. |

## JSON Report Output

The CLI now supports JSON report generation for automation.

```bash
dotnet run --project src/WebShield.Cli -- scan https://example.com --json reports/example.json
```

JSON output includes:

- Target URL
- Start and finish timestamps
- Score
- Severity summary
- Ordered findings
- Check ID, title, severity, description, impact, recommendation, and evidence for each finding

## CI/CD Quality Gate

The CLI now supports configurable fail thresholds.

Default behavior:

```bash
dotnet run --project src/WebShield.Cli -- scan https://example.com
```

The default fail threshold is `High`. If any High or Critical finding is present, the CLI returns exit code `2`.

Custom threshold:

```bash
dotnet run --project src/WebShield.Cli -- scan https://example.com --fail-on Medium
```

Allowed severity values:

- `Info`
- `Low`
- `Medium`
- `High`
- `Critical`

Report-only mode:

```bash
dotnet run --project src/WebShield.Cli -- scan https://example.com --json reports/example.json --no-fail
```

This mode always returns exit code `0` after a completed scan while still printing and writing findings.

## Safety Boundaries

These upgrades remain non-destructive and defensive. They do not add:

- Exploitation logic
- Credential attacks
- Brute force behavior
- WAF bypass or evasion
- Payload fuzzing
- Mass scanning
- Secret extraction

## Recommended Next Upgrades

Good next steps:

1. HTML report export
2. SARIF output for GitHub code scanning
3. OpenAPI schema parsing
4. Local safe testing lab sample app
5. Platform-specific remediation snippets
6. GitHub Actions reusable workflow example
7. Release packaging for CLI binaries
