# Manual Testing Guide

This guide explains how to manually test WebShield Security Scanner during development, release preparation, and product validation.

Manual testing is important because WebShield is a developer-facing security tool. The scanner must be technically correct, but it must also feel safe, predictable, and useful to a real developer or client.

## Manual Testing Rules

Only test against targets you own or are explicitly authorized to test.

Recommended targets:

- Local development applications
- Local Docker containers
- Internal staging environments with written permission
- Intentionally vulnerable training apps running locally
- Demo sites created specifically for WebShield validation

Avoid:

- Third-party production sites
- Unknown public targets
- Mass scanning
- Login brute force
- Exploit payload testing
- Any test that may disrupt availability or confidentiality

## Pre-Test Checklist

Before running manual tests:

- [ ] Confirm the target is owned or authorized.
- [ ] Confirm the target URL is correct.
- [ ] Confirm the scan is safe and non-destructive.
- [ ] Confirm no credentials or secrets will be logged.
- [ ] Confirm where reports will be written.
- [ ] Confirm the expected behavior of the target.

## Basic CLI Test

Run:

```bash
dotnet run --project src/WebShield.Cli -- scan https://example.com
```

Expected result:

- CLI starts successfully.
- Target URL is printed.
- Score is printed.
- Findings are grouped by severity in readable output.
- No unhandled exception appears.

## Markdown Report Test

Run:

```bash
dotnet run --project src/WebShield.Cli -- scan https://example.com --report reports/example.md
```

Expected result:

- `reports/example.md` is created.
- Report includes target URL.
- Report includes started and finished timestamps.
- Report includes score.
- Report includes findings.
- Each finding includes description, impact, recommendation, and evidence.
- Report does not contain secrets or excessive response bodies.

## Timeout Test

Run against a slow local endpoint or controlled test server:

```bash
dotnet run --project src/WebShield.Cli -- scan http://localhost:5000/slow --timeout 3
```

Expected result:

- Scanner exits gracefully.
- Output explains that the target could not be reached or timed out.
- Report generation still works if requested.
- No stack trace is shown in normal user output.

## Invalid URL Test

Run:

```bash
dotnet run --project src/WebShield.Cli -- scan not-a-url
```

Expected result:

- CLI returns a non-zero exit code.
- User receives a clear invalid URL message.
- No scan starts.

## Transport Security Test

Use a local app or owned staging environment that can be reached over HTTP and HTTPS.

Test cases:

| Target behavior | Expected scanner result |
|---|---|
| Target URL starts with `https://` | Informational finding: `HTTPS is enabled for the target URL` |
| Target URL starts with `http://` and redirects to `https://` | Informational finding: `HTTP redirects to HTTPS` |
| Target URL starts with `http://` and does not redirect to HTTPS | Medium finding: `Target does not use HTTPS` |

Expected evidence:

- Initial target URL is shown.
- Final URL is shown when available.
- No certificate details, secrets, or response body content are stored.

## HTTP Security Header Test

Use a local server with intentionally missing security headers.

Expected findings when missing:

- `Missing Content-Security-Policy`
- `Missing Strict-Transport-Security` for HTTPS targets
- `Missing X-Frame-Options`
- `Missing X-Content-Type-Options`
- `Missing Referrer-Policy`
- `Missing Permissions-Policy`

Expected behavior when headers are present:

- Missing-header warnings should not appear for present headers.
- The scanner may still add an informational finding.

## Cookie Flag Test

Use a local app that sets cookies with different attributes.

Test cases:

| Cookie configuration | Expected behavior |
|---|---|
| No `Set-Cookie` header | Informational finding only |
| Missing `Secure` on HTTPS | Medium finding |
| Missing `HttpOnly` | Medium finding |
| Missing `SameSite` | Low finding |
| Has `Secure`, `HttpOnly`, and `SameSite` | No cookie warning for that cookie |

## Swagger/OpenAPI Exposure Test

Use a local app that exposes Swagger in development mode.

Common endpoints checked:

- `/swagger`
- `/swagger/index.html`
- `/swagger/v1/swagger.json`
- `/openapi.json`

Expected result when endpoint returns HTTP 200:

- Scanner reports possible public API documentation exposure.
- Finding severity is Medium.
- Recommendation tells the user to restrict documentation in production or require authentication.

Expected result when endpoints return HTTP 404:

- Scanner reports that common Swagger/OpenAPI endpoints were not publicly reachable.

## Sensitive File Exposure Test

Use a controlled local server only. Do not test unknown public targets.

Common paths checked:

- `/.env`
- `/.git/config`
- `/backup.zip`
- `/database.sql`

Expected result when a path returns HTTP 200:

- Scanner reports potential sensitive file exposure.
- Finding severity is High.
- Evidence shows the reachable endpoint.

Expected result when paths return HTTP 403 or 404:

- No High severity sensitive file exposure finding should appear.

## Report Review Checklist

After generating a report, verify:

- [ ] The title is clear.
- [ ] The target is correct.
- [ ] The score is present.
- [ ] Findings are ordered by severity.
- [ ] Each finding has practical remediation.
- [ ] Evidence is concise.
- [ ] The report explains that it is not a full penetration test.
- [ ] The report is suitable to share with a developer or client.

## Release Candidate Manual QA

Before tagging a release:

- [ ] Run `dotnet restore`.
- [ ] Run `dotnet build`.
- [ ] Run `dotnet test`.
- [ ] Run CLI help.
- [ ] Run a scan against a safe local target.
- [ ] Generate a Markdown report.
- [ ] Check exit codes.
- [ ] Review README quick start.
- [ ] Review `SECURITY.md` and legal/ethical use docs.
- [ ] Confirm no offensive features were introduced.

## Manual QA Notes Template

Use this template when testing a release candidate:

```text
Version/commit:
Tester:
Date:
Target type: local / staging / demo
Target URL:
Authorization confirmed: yes/no
Command used:
Expected result:
Actual result:
Exit code:
Report path:
Issues found:
Follow-up required:
```
