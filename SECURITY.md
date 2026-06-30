# Security Policy

## Responsible Use

WebShield Security Scanner is intended for defensive security testing only.

You may use this project to test:

- Web applications you own
- Internal systems where you have explicit authorization
- Local development environments
- Training labs and intentionally vulnerable demo applications

Do not use this project for:

- Testing third-party systems without permission
- Credential attacks or brute force attempts
- Exploitation, persistence, or post-exploitation activity
- WAF bypass or evasion research against public targets
- Mass scanning the public internet

## Supported Versions

This repository is currently in MVP development. Security fixes should target the `main` branch until versioned releases are introduced.

## Reporting Security Issues

For now, open a private GitHub security advisory if available, or contact the repository owner directly before publishing details.

When reporting an issue, include:

- Affected version or commit
- Reproduction steps
- Expected behavior
- Actual behavior
- Suggested remediation, if known

## Safe Design Principles

WebShield should remain:

- Non-destructive by default
- Rate-limited by design
- Focused on misconfiguration and defensive reporting
- Clear about authorization requirements
- Transparent in all network requests it performs
