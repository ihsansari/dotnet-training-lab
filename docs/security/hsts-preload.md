# HSTS preload (hstspreload.org) — policy note

## What it is
HSTS uses the `Strict-Transport-Security` header to tell browsers: "always use HTTPS for this domain".
Preloading goes further: browsers ship a built-in list so HTTPS is enforced even on first visit.

## Why it matters
Preload reduces first-visit downgrade risk, but it is a long-term operational commitment.

## Preload requirements (high-level)
To be eligible for preload, your site must send `Strict-Transport-Security` with:
- `max-age >= 31536000` (1 year)
- `includeSubDomains`
- `preload`

And HTTPS must work for the apex domain and all subdomains.

## Risks / caution
- If any subdomain cannot serve HTTPS later, users may be blocked (browsers will force HTTPS).
- Removal from the preload list can take time to propagate.

## ASP.NET Core notes
- Setting `options.Preload = true` adds the `preload` directive to the header.
- This does NOT automatically add your domain to the browser preload list; you still must meet requirements and submit.

Example (only if you commit to preload as a policy):
- AddHsts: Preload=true, IncludeSubDomains=true, MaxAge>=365 days
- UseHsts: enabled only outside Development
