# Kestrel limits (DoS hardening)

## What we set
- RequestHeadersTimeout = 15s
- MaxRequestHeadersTotalSize = 16 KB
- MaxRequestBodySize = 10 MB

## Why it matters (security)
Slowloris attacks keep many connections open by sending HTTP headers very slowly. Tightening the header receive timeout reduces the attacker’s ability to hold connections open. Size limits reduce resource abuse from oversized headers/bodies and make API behavior predictable.

## Defaults / notes
- Kestrel defaults: RequestHeadersTimeout ~30s, headers total size 32KB, request body size ~28.6MB.
- MaxRequestBodySize can be overridden per-request when needed (e.g., specific endpoints that legitimately accept larger uploads).
