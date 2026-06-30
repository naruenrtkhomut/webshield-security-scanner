# Security Checks

This document defines the first safe checks for WebShield.

## Transport Security

WebShield checks whether the target uses HTTPS or whether an HTTP target redirects to HTTPS during the baseline request.

| Condition | Finding | Initial severity |
|---|---|---|
| Target URL uses `https://` | HTTPS is enabled for the target URL | Info |
| Target URL uses `http://` and final URL is `https://` | HTTP redirects to HTTPS | Info |
| Target URL uses `http://` and final URL is still `http://` or unknown | Target does not use HTTPS | Medium |

This check does not inspect certificate chains or perform TLS protocol probing yet. It is a baseline transport security signal for developer-friendly reports.

## HTTP Security Headers

| Header | Risk when missing | Initial severity |
|---|---|---|
| `Content-Security-Policy` | Higher impact from XSS and content injection | High |
| `Strict-Transport-Security` | HTTPS downgrade or insecure repeat visits | Medium |
| `X-Frame-Options` | Clickjacking exposure | Medium |
| `X-Content-Type-Options` | MIME sniffing risk | Low |
| `Referrer-Policy` | Sensitive URL leakage via referrer | Low |
| `Permissions-Policy` | Browser features not explicitly restricted | Low |

## Cookie Flags

For every `Set-Cookie` header, WebShield checks:

- `Secure`
- `HttpOnly`
- `SameSite`

## CORS Policy

WebShield inspects baseline CORS response headers:

- `Access-Control-Allow-Origin`
- `Access-Control-Allow-Credentials`

| Condition | Finding | Initial severity |
|---|---|---|
| No baseline CORS headers | No CORS headers observed on homepage response | Info |
| `Access-Control-Allow-Origin: *` with credentials enabled | Wildcard CORS origin allows credentials | High |
| `Access-Control-Allow-Origin: *` without credentials | Wildcard CORS origin observed | Low |
| Null origin appears allowed | CORS policy allows null origin | Medium |
| Explicit non-wildcard CORS headers | CORS headers observed | Info |

This is a baseline signal. API endpoints may have different CORS behavior and should be reviewed separately in authenticated and API-specific flows.

## Information Disclosure Headers

WebShield checks for common headers that may reveal server, framework, or generator details:

- `Server`
- `X-Powered-By`
- `X-AspNet-Version`
- `X-AspNetMvc-Version`
- `X-Generator`

| Condition | Finding | Initial severity |
|---|---|---|
| Disclosure header is present | Technology disclosure header observed | Low |
| No common disclosure headers are present | No common technology disclosure headers observed | Info |

## Cache Policy

WebShield inspects the baseline response caching policy.

| Condition | Finding | Initial severity |
|---|---|---|
| Missing `Cache-Control` | Missing Cache-Control header | Low |
| Response sets cookies and appears publicly cacheable | Cookie-setting response may be publicly cacheable | Medium |
| Explicit `Cache-Control` is present without the risky cookie/public combination | Cache-Control header observed | Info |

Cache findings are baseline hardening signals. Sensitive, authenticated, and user-specific routes should be reviewed separately.

## Swagger/OpenAPI Exposure

WebShield checks common documentation endpoints:

- `/swagger`
- `/swagger/index.html`
- `/swagger/v1/swagger.json`
- `/openapi.json`

This check does not exploit the API. It only reports whether public API documentation appears reachable.

## Sensitive File Exposure

WebShield checks a small baseline set of common sensitive paths:

- `/.env`
- `/.git/config`
- `/backup.zip`
- `/database.sql`

These checks must stay low-volume and transparent. Evidence should report the reachable URL only, not the full file contents.

## Severity Guide

| Severity | Meaning |
|---|---|
| Info | Useful observation |
| Low | Hardening issue |
| Medium | Meaningful misconfiguration |
| High | Serious issue that should be fixed before production |
| Critical | Severe issue that can expose sensitive systems or data |
