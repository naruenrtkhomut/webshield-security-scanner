# Test Case Matrix

This document defines the baseline test cases for WebShield Security Scanner.

The matrix should be updated whenever a new security check, report format, CLI option, or scoring rule is added.

## Legend

| Field | Meaning |
|---|---|
| Area | Feature or module under test |
| Scenario | Condition being tested |
| Expected Result | What should happen |
| Type | Unit, component, integration, manual, or regression |
| Priority | P0 critical, P1 important, P2 useful |

## Scanner Core

| ID | Area | Scenario | Expected Result | Type | Priority |
|---|---|---|---|---|---|
| CORE-001 | URL validation | Target uses `https://` | Scan starts | Unit | P0 |
| CORE-002 | URL validation | Target uses `http://` | Scan starts | Unit | P0 |
| CORE-003 | URL validation | Target uses unsupported scheme | Scanner rejects target | Unit | P0 |
| CORE-004 | Reachability | Target cannot be reached | High finding is added and scan does not crash | Component | P0 |
| CORE-005 | Reachability | Target returns HTTP 200 | Checks receive homepage response | Component | P0 |
| CORE-006 | Reachability | Target redirects | Scanner follows normal HttpClient behavior | Integration | P1 |
| CORE-007 | Cancellation | Cancellation token is triggered | Operation stops without swallowing cancellation | Unit | P1 |
| CORE-008 | Timeout | Target times out | CLI shows controlled failure and exits safely | Manual | P1 |

## Scoring

| ID | Area | Scenario | Expected Result | Type | Priority |
|---|---|---|---|---|---|
| SCORE-001 | Score calculation | No findings | Score is 100 | Unit | P1 |
| SCORE-002 | Score calculation | One Low finding | Score loses 5 points | Unit | P1 |
| SCORE-003 | Score calculation | One Medium finding | Score loses 15 points | Unit | P1 |
| SCORE-004 | Score calculation | One High finding | Score loses 25 points | Unit | P1 |
| SCORE-005 | Score calculation | One Critical finding | Score loses 40 points | Unit | P1 |
| SCORE-006 | Score calculation | Many findings exceed 100 penalty | Score does not go below 0 | Unit | P0 |

## HTTP Security Headers

| ID | Area | Scenario | Expected Result | Type | Priority |
|---|---|---|---|---|---|
| HDR-001 | Content-Security-Policy | Header is missing | High finding is returned | Unit | P0 |
| HDR-002 | Content-Security-Policy | Header is present | Missing CSP finding is not returned | Unit | P0 |
| HDR-003 | Strict-Transport-Security | HTTPS target and header is missing | Medium finding is returned | Unit | P0 |
| HDR-004 | Strict-Transport-Security | HTTP target and header is missing | HSTS missing finding is not returned | Unit | P1 |
| HDR-005 | X-Frame-Options | Header is missing | Medium finding is returned | Unit | P1 |
| HDR-006 | X-Content-Type-Options | Header is missing | Low finding is returned | Unit | P1 |
| HDR-007 | Referrer-Policy | Header is missing | Low finding is returned | Unit | P1 |
| HDR-008 | Permissions-Policy | Header is missing | Low finding is returned | Unit | P1 |
| HDR-009 | All baseline headers | All headers are present | Informational success finding is returned | Unit | P1 |
| HDR-010 | Response unavailable | Homepage response is null | Informational skipped finding is returned | Unit | P0 |

## Cookie Security

| ID | Area | Scenario | Expected Result | Type | Priority |
|---|---|---|---|---|---|
| COOKIE-001 | Cookie headers | No `Set-Cookie` header | Informational finding only | Unit | P1 |
| COOKIE-002 | Secure flag | HTTPS target and cookie is missing `Secure` | Medium finding is returned | Unit | P0 |
| COOKIE-003 | Secure flag | HTTP target and cookie is missing `Secure` | Secure missing finding is not required | Unit | P1 |
| COOKIE-004 | HttpOnly flag | Cookie is missing `HttpOnly` | Medium finding is returned | Unit | P0 |
| COOKIE-005 | SameSite attribute | Cookie is missing `SameSite` | Low finding is returned | Unit | P1 |
| COOKIE-006 | Secure cookie | Cookie has `Secure`, `HttpOnly`, and `SameSite` | No warning for that cookie | Unit | P0 |
| COOKIE-007 | Multiple cookies | One secure cookie and one weak cookie | Finding only for weak cookie | Unit | P1 |
| COOKIE-008 | Attribute casing | Cookie uses lowercase attributes | Attributes are still recognized | Unit | P1 |

## Swagger/OpenAPI Exposure

| ID | Area | Scenario | Expected Result | Type | Priority |
|---|---|---|---|---|---|
| API-001 | Swagger path | `/swagger` returns HTTP 200 | Medium finding is returned | Component | P0 |
| API-002 | Swagger UI | `/swagger/index.html` returns HTTP 200 | Medium finding is returned | Component | P0 |
| API-003 | Swagger JSON | `/swagger/v1/swagger.json` returns HTTP 200 | Medium finding is returned | Component | P0 |
| API-004 | OpenAPI JSON | `/openapi.json` returns HTTP 200 | Medium finding is returned | Component | P0 |
| API-005 | Not found | All common paths return HTTP 404 | Informational no-exposure finding is returned | Component | P1 |
| API-006 | Forbidden | Paths return HTTP 403 | No exposure finding is returned | Component | P1 |
| API-007 | Network error | Request to optional endpoint fails | Scan continues | Component | P0 |
| API-008 | Cancellation | Cancellation occurs during optional endpoint check | Cancellation is rethrown | Unit | P1 |

## Sensitive File Exposure

| ID | Area | Scenario | Expected Result | Type | Priority |
|---|---|---|---|---|---|
| FILE-001 | `.env` exposure | `/.env` returns HTTP 200 | High finding is returned | Component | P0 |
| FILE-002 | Git config exposure | `/.git/config` returns HTTP 200 | High finding is returned | Component | P0 |
| FILE-003 | Backup exposure | `/backup.zip` returns HTTP 200 | High finding is returned | Component | P0 |
| FILE-004 | Database dump exposure | `/database.sql` returns HTTP 200 | High finding is returned | Component | P0 |
| FILE-005 | Not found | All sensitive file paths return HTTP 404 | Informational no-exposure finding is returned | Component | P1 |
| FILE-006 | Forbidden | Paths return HTTP 403 | No exposure finding is returned | Component | P1 |
| FILE-007 | Network error | Optional path request fails | Scan continues | Component | P0 |
| FILE-008 | Evidence | A sensitive file path is reachable | Evidence includes URL only, not file content | Component | P0 |

## CLI Behavior

| ID | Area | Scenario | Expected Result | Type | Priority |
|---|---|---|---|---|---|
| CLI-001 | Help | Run without args | Usage is printed and exit code is 0 | Integration | P1 |
| CLI-002 | Unknown command | Run unsupported command | Error and usage are printed | Integration | P1 |
| CLI-003 | Missing target | Run `scan` without URL | Error and usage are printed | Integration | P0 |
| CLI-004 | Invalid target | Run `scan not-a-url` | Invalid URL message is printed | Integration | P0 |
| CLI-005 | Valid target | Run scan against safe target | Results are printed | Integration | P0 |
| CLI-006 | High findings | Scan has High finding | Exit code is 2 | Integration | P1 |
| CLI-007 | No High findings | Scan has Info/Low/Medium only | Exit code is 0 | Integration | P1 |
| CLI-008 | Report option | Use `--report reports/example.md` | Markdown report is written | Integration | P0 |
| CLI-009 | Timeout option | Use `--timeout 3` | HttpClient timeout is applied | Manual | P1 |

## Reporting

| ID | Area | Scenario | Expected Result | Type | Priority |
|---|---|---|---|---|---|
| REPORT-001 | Markdown title | Generate report | Report has WebShield title | Unit | P1 |
| REPORT-002 | Target metadata | Generate report | Target URL appears | Unit | P1 |
| REPORT-003 | Timestamps | Generate report | Started and finished timestamps appear | Unit | P1 |
| REPORT-004 | Score | Generate report | Score appears as `x/100` | Unit | P1 |
| REPORT-005 | Finding details | Finding exists | Title, severity, check ID, description, impact, recommendation, evidence appear | Unit | P0 |
| REPORT-006 | Ordering | Multiple findings exist | Findings are ordered by severity | Unit | P1 |
| REPORT-007 | Disclaimer | Generate report | Report includes defensive scanner disclaimer | Unit | P1 |
| REPORT-008 | Secret handling | Evidence contains sensitive-looking content | Report should avoid storing full secret-bearing response bodies | Unit | P0 |

## Documentation and Product Readiness

| ID | Area | Scenario | Expected Result | Type | Priority |
|---|---|---|---|---|---|
| DOC-001 | README | User wants quick start | README includes restore, build, run commands | Manual | P0 |
| DOC-002 | Responsible use | User wants scope | README and SECURITY.md explain authorized testing only | Manual | P0 |
| DOC-003 | Contributing | Contributor wants to add check | CONTRIBUTING.md explains defensive scope | Manual | P1 |
| DOC-004 | Sponsorship | Potential sponsor wants to support | README links sponsorship docs | Manual | P1 |
| DOC-005 | Testing | Contributor wants to test | README links testing docs | Manual | P1 |

## Acceptance Criteria for CI

The CI test suite should eventually include all P0 and P1 unit/component tests. Manual tests remain documented until they can be automated safely and deterministically.

Minimum release gate:

- All P0 tests pass.
- All P1 tests pass or have documented exceptions.
- CLI can generate a report.
- No test depends on unauthorized public targets.
- No test adds offensive behavior.
