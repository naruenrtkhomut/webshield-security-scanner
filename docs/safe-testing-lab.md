# Safe Testing Lab Guide

This guide describes how to validate WebShield Security Scanner safely without testing unauthorized systems.

The lab is intentionally focused on defensive validation. It should help maintainers verify scanner behavior without introducing exploit workflows, credential attacks, stealth behavior, or destructive testing.

## Lab Goals

A safe WebShield lab should let contributors test:

- Missing and present HTTP security headers
- Secure and insecure cookie attributes
- Public and restricted Swagger/OpenAPI endpoints
- Public and blocked sensitive file paths
- CLI output
- Markdown report output
- Error handling
- Timeout handling
- Exit codes

## Lab Boundaries

The lab must not include:

- Real credentials
- Real customer data
- Public target scanning
- Exploit chains
- Brute-force endpoints
- WAF bypass exercises
- Persistence or post-exploitation behavior
- Anything that could be mistaken for offensive tooling

## Recommended Local Lab Structure

A future local test lab can live under:

```text
samples/
└── WebShield.TestTarget/
    ├── Program.cs
    ├── WebShield.TestTarget.csproj
    └── README.md
```

The sample app should expose predictable endpoints for scanner validation.

## Suggested Local Endpoints

| Endpoint | Purpose | Expected scanner behavior |
|---|---|---|
| `/` | Baseline homepage | Security header and cookie checks inspect this response |
| `/secure-headers` | Response with all baseline headers | Missing-header findings should not appear |
| `/missing-headers` | Response with no hardening headers | Missing-header findings should appear |
| `/cookie-secure` | Sets secure cookie flags | Cookie warnings should not appear for that cookie |
| `/cookie-insecure` | Sets weak cookie flags | Cookie warnings should appear |
| `/swagger/index.html` | Simulates public Swagger UI | Swagger exposure finding should appear |
| `/openapi.json` | Simulates public OpenAPI document | Swagger/OpenAPI exposure finding should appear |
| `/.env` | Simulates accidental sensitive file exposure | Sensitive file exposure finding should appear |
| `/slow` | Delays response | Timeout behavior can be tested |
| `/error` | Returns HTTP 500 | Scanner should not crash |

## Example Local Test Flow

Start the local test target:

```bash
dotnet run --project samples/WebShield.TestTarget
```

Run WebShield:

```bash
dotnet run --project src/WebShield.Cli -- scan http://localhost:5000 --report reports/local-lab.md
```

Expected result:

- Scan completes.
- Findings match intentionally configured endpoints.
- Report is generated.
- No real secrets are created or exposed.

## Controlled Sensitive File Simulation

When simulating sensitive files, never use real `.env`, `.git/config`, backup, or database dump contents.

Use harmless placeholder content such as:

```text
THIS_IS_FAKE_TEST_CONTENT=not-a-secret
```

The scanner should only report that a sensitive-looking path is reachable. It should not store or print full file contents.

## Test Data Rules

Use fake values only:

- `FAKE_API_KEY_FOR_TESTING_ONLY`
- `fake@example.test`
- `test-session-id`
- `dummy-token-value`

Do not use:

- Real API keys
- Real session IDs
- Real database strings
- Production URLs
- Customer data
- Internal secrets

## Localhost First Policy

New scanner behavior should be tested against localhost or mocked handlers first.

Only after local validation should a maintainer run manual tests against an owned staging system.

## Safe Request Policy

The lab should validate that WebShield uses safe request behavior:

- Low request volume
- Clear user-agent
- No credential submission
- No form submission
- No destructive HTTP methods
- No payload-based exploitation
- No evasion behavior

## Future Docker Lab

A future Docker-based lab can include:

```text
docker-compose.yml
samples/
  WebShield.TestTarget/
```

Desired commands:

```bash
docker compose up --build

dotnet run --project src/WebShield.Cli -- scan http://localhost:8080 --report reports/docker-lab.md
```

The Docker lab should be optional and must not be required for the basic unit test suite.

## Lab Acceptance Criteria

The lab is acceptable when:

- It runs fully on localhost.
- It contains no real secrets.
- It has predictable responses.
- It supports both positive and negative scanner cases.
- It documents every intentionally weak endpoint.
- It reinforces authorized defensive testing.
