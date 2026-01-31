## HSTS preload (hstspreload.org) — optional, high commitment

HSTS (`Strict-Transport-Security`) is enabled to force browsers to use HTTPS. If we also choose **HSTS preloading**, the domain can be shipped in the browser preload list, reducing “first-visit downgrade” risk. This is optional and must be treated as an operational commitment.

### Requirements (to be eligible for preload)
- `Strict-Transport-Security` must include:
  - `max-age >= 31536000` (1 year)
  - `includeSubDomains`
  - `preload`
- HTTPS must work for the apex domain and **all subdomains**, and redirects must also send the HSTS header.

### Risk / caution
- Sending `preload` can have long-lasting impact. If any subdomain can’t serve HTTPS later, users may be blocked.
- Removal from the preload list can take weeks to propagate to users.

Reference: hstspreload.org (submission rules and removal guidance).
