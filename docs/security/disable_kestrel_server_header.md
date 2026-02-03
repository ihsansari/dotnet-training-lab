# Disable Kestrel "Server" header (defense-in-depth)

## What changes
By default, Kestrel can add a response header like:
- `Server: Kestrel`

We disable it via:
- `KestrelServerOptions.AddServerHeader = false`

This prevents Kestrel from including the `Server` header in responses. (It only affects Kestrel; reverse proxies may still add their own `Server` header.)

## Why this matters (security)
This is **defense-in-depth** against reconnaissance:
- During information gathering, attackers fingerprint the web stack using HTTP responses and headers.
- Knowing the server/framework helps an attacker tailor scans and exploit attempts to the most likely tech/vulns, reducing their cost and increasing success rate.

OWASP explicitly documents web server fingerprinting as an information-gathering technique. The `Server` header is one of the obvious signals.  

## How attackers use it (concrete)
Typical recon flow:
1) Send a cheap request (`HEAD` / `GET`) and collect response headers.
2) If `Server` reveals stack (e.g., Kestrel/IIS/nginx), the attacker:
   - prioritizes tool modules and checks specific to that server family,
   - tunes payloads/timeouts/HTTP quirks,
   - focuses on known misconfigs common to that stack.

Even if fingerprinting is possible by other means, removing unnecessary detail reduces "easy wins" and noisy stack leakage.

## Verify (before/after)
Run:
- `curl -skI https://localhost:5001/health`

Before (example):
- `Server: Kestrel`

After:
- no `Server` header from Kestrel.

## Limitations / guardrails
- This does **not** replace patching: keeping your runtime and dependencies updated is more important than header hiding.
- If you deploy behind IIS/nginx/CDN/WAF, that layer may still emit `Server` (configure there separately if required).
