# Testing Strategy

WebShield Security Scanner must be tested as both a .NET application and a defensive security product. The goal is not only to verify that code works, but also to verify that every check remains safe, predictable, explainable, and suitable for authorized testing.

## Testing Goals

The test suite should prove that WebShield:

- Detects common web security misconfigurations accurately.
- Avoids destructive behavior.
- Produces clear findings with severity, impact, recommendation, and evidence.
- Generates reports that are useful for developers and clients.
- Handles network errors, redirects, timeouts, and unusual HTTP responses gracefully.
- Can be used in CI/CD without noisy or unstable behavior.

## Testing Principles

### 1. Safety First

Every automated and manual test must stay within the defensive scope of the project. Tests should use local applications, mocked HTTP handlers, controlled demo services, or systems where the tester has explicit authorization.

WebShield tests must not require:

- Exploit payloads
- Credential attacks
- Brute forcing
- WAF bypass
- Stealth behavior
- Public internet mass scanning
- Destructive requests

### 2. Deterministic by Default

Unit tests should not depend on external websites. Any test that requires a live HTTP target should be classified as manual or integration-only.

The default CI pipeline should run quickly and consistently using:

- In-memory objects
- Mocked `HttpMessageHandler` implementations
- Local test servers
- Fixed sample responses

### 3. Clear Finding Quality

A finding is only useful if a developer can understand and fix it. Tests should validate not only that a finding exists, but also that it includes:

- Stable `CheckId`
- Accurate title
- Correct severity
- Clear description
- Practical impact statement
- Actionable recommendation
- Evidence that explains why the finding was raised

### 4. Low Noise

Security scanners lose trust when they produce excessive false positives. Each check should include tests for both positive and negative cases.

For every new check, add tests for:

- Misconfigured target should produce a finding.
- Correctly configured target should not produce the same warning.
- Unreachable or unusual responses should not crash the scan.
- Evidence should not expose secrets or large response bodies.

## Test Layers

## Unit Tests

Unit tests verify isolated logic without real network dependencies.

Current unit test priorities:

- `ScanResult` scoring
- Security header detection
- Cookie flag detection
- Markdown report generation
- URL validation
- Finding ordering and severity behavior

Unit tests should run with:

```bash
dotnet test
```

## Component Tests

Component tests verify a full check or scanner flow using a mocked HTTP pipeline.

Examples:

- Run `SecurityHeadersCheck` against a fake response with no headers.
- Run `CookieSecurityCheck` against a response with insecure cookies.
- Run `SwaggerExposureCheck` against a fake handler that returns HTTP 200 for `/swagger`.
- Run `SensitiveFileExposureCheck` against a fake handler that returns HTTP 404 for all sensitive paths.

## Integration Tests

Integration tests may use a local HTTP server or intentionally vulnerable local demo app.

Integration tests should verify:

- CLI command execution
- Report file creation
- Timeout handling
- Redirect behavior
- HTTPS vs HTTP behavior
- End-to-end scan result structure

Integration tests should not be required for every CI run until they are stable and isolated.

## Manual QA Tests

Manual QA verifies real developer experience:

- Is the command easy to run?
- Is the output readable?
- Is the report useful for a client or engineering team?
- Are recommendations practical?
- Does the tool explain its defensive scope clearly?

Manual QA should use local applications, owned staging environments, or explicit authorized targets only.

## Regression Tests

When fixing a bug, add a regression test that fails before the fix and passes after it.

Examples:

- Header check incorrectly treats lowercase headers as missing.
- Cookie parsing fails when attributes appear in a different order.
- Scanner crashes when a target returns no content.
- Report writer fails when evidence contains backticks or new lines.

## CI Testing

The CI pipeline should keep three core commands healthy:

```bash
dotnet restore WebShield.sln
dotnet build WebShield.sln --configuration Release --no-restore
dotnet test WebShield.sln --configuration Release --no-build --verbosity normal
```

Future CI improvements:

- Code coverage report
- Static analysis
- Markdown linting
- Security dependency audit
- CLI smoke test against a local test server
- Release package validation

## Quality Gates

A pull request should not be merged if it:

- Adds offensive or destructive behavior.
- Adds a check without tests.
- Produces findings without clear remediation text.
- Makes live external network tests required for CI.
- Leaks sensitive response content into logs or reports.
- Makes scanner behavior non-deterministic without a documented reason.

## Definition of Done for a New Check

A new check is complete when it has:

- Stable `CheckId`
- Defensive purpose
- Safe request pattern
- Positive test case
- Negative test case
- Error-handling test case
- Clear finding title
- Correct severity
- Practical recommendation
- Documentation in `docs/security-checks.md` or a related document
- Manual QA notes if behavior depends on real web server configuration

## Recommended Test Naming

Use descriptive test names that explain the behavior:

```csharp
[Fact]
public async Task RunAsync_WhenContentSecurityPolicyIsMissing_ShouldReturnHighSeverityFinding()
{
    // Arrange
    // Act
    // Assert
}
```

Prefer behavior names over implementation names.

## Long-Term Testing Roadmap

1. Add mocked HTTP handler utilities.
2. Add full tests for cookie flags.
3. Add full tests for Swagger/OpenAPI exposure.
4. Add full tests for sensitive file exposure.
5. Add Markdown report tests.
6. Add CLI smoke tests.
7. Add local demo app for integration tests.
8. Add code coverage reporting.
9. Add quality gate mode tests.
10. Add release artifact tests.
