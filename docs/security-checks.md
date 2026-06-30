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
