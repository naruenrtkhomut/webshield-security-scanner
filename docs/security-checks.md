# Security Checks

This document defines the first safe checks for WebShield.

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

Future safe checks may include:

- `/.env`
- `/.git/config`
- `/backup.zip`
- `/database.sql`

These checks must stay low-volume and transparent.

## Severity Guide

| Severity | Meaning |
|---|---|
| Info | Useful observation |
| Low | Hardening issue |
| Medium | Meaningful misconfiguration |
| High | Serious issue that should be fixed before production |
| Critical | Severe issue that can expose sensitive systems or data |
