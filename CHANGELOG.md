# Changelog

This repository is a training lab, but the baseline is **security-first**:
reproducible builds + supply-chain hygiene (fast patching of known vulnerabilities).

## 2026-01-31 — Root security baseline

### Added

- `global.json`
  - **Why**: Pins the .NET SDK used by developers and CI.
  - **Security impact**: Reduces “works on my machine” drift and makes it easier to roll out patched SDKs consistently (same toolchain everywhere).

- `Directory.Build.props`
  - **Why**: Central place for repo-wide MSBuild rules (applies automatically to projects under the folder).
  - **Security impact**: Enables **NuGet auditing** and (in CI) escalates vulnerability warnings (NU1902–NU1904) to errors so vulnerable restores can’t silently ship.

- `Directory.Packages.props`
  - **Why**: Central Package Management (CPM) keeps all package versions in one file.
  - **Security impact**: Faster and safer upgrades (one place to patch versions), reduces dependency drift across projects.

- `.github/dependabot.yml`
  - **Why**: Automates dependency update PRs (NuGet + GitHub Actions).
  - **Security impact**: Shortens the time-to-patch for supply-chain issues by continuously proposing updates. Note: security updates are tied to the default branch behavior, so keep branch strategy in mind.

- `ST.Security.slnx`
  - **Why**: Stable entry point for restore/build across tools and CI.
  - **Security impact**: Makes it easy to run consistent restore/audit/build checks as the repo grows (single command surface).

- `.github/workflows/ci.yml`
  - **Why**: Runs `restore` + `build` on every PR/push to `develop` using the SDK pinned in `global.json`.
  - **Security impact**: Enforces dependency vulnerability auditing in CI (restore warnings NU1902–NU1904) and blocks merges when moderate/high/critical vulnerable packages are present, instead of relying on local dev machines.
  - **Implementation**: Uses `actions/setup-dotnet` with `global-json-file`, and builds with `-p:ContinuousIntegrationBuild=true` so repo rules can treat vulnerability warnings as errors.

### Why we enforce restore + build like this (security)

We run:

- `dotnet restore ST.Security.slnx -warnaserror:NU1902,NU1903,NU1904`
- `dotnet build ST.Security.slnx -c Release --no-restore -p:ContinuousIntegrationBuild=true`

**What we gain:** `dotnet restore` emits vulnerability warnings by default on .NET 8+ SDKs, and `NU1902–NU1904` correspond to **moderate / high / critical** known vulnerabilities. Turning them into errors makes vulnerable dependencies a **hard CI gate** (the job fails, so the PR cannot be merged).

`--no-restore` ensures the build step does not perform an implicit restore (which could otherwise re-run dependency resolution) and keeps **restore as the single enforced policy point**. Setting `ContinuousIntegrationBuild=true` enables CI-only build behavior recommended for official builds and lets repo rules apply consistently in CI.


## 2026-01-31 — PR dependency review gate

### Added

- `.github/workflows/dependency-review.yml`
  - **Why**: Reviews dependency changes introduced by each pull request.
  - **Security impact**: Fails the PR if it introduces dependencies with known vulnerabilities at or above the configured severity threshold (here: moderate). This makes “new vulnerable dependency introduced by a PR” a hard merge blocker.

## 2026-01-31 — NuGet source mapping baseline

### Added

- `nuget.config`
  - **Why**: Explicitly defines the allowed package source(s) and uses Package Source Mapping.
  - **Security impact**: Improves supply-chain security and determinism by controlling which source(s) NuGet will search for packages (especially important once private feeds exist).

## 2026-01-31 — First .NET 10 project

### Added

- `src/ST.Security.Api/*`
  - **Why**: Establishes a real `net10.0` project with a stable, unique `ST.*` name and integrates it into the `.slnx`.
  - **Security impact**: Ensures CI/tooling/auditing run against an actual .NET 10 target early (reduces surprises later) and sets the baseline for HTTPS/HSTS-by-default API hardening.

- `src/ST.Security.Api/Program.cs`
  - **Why these defaults / what they do (and which headers you get)**:
    - `app.UseHsts()` (only outside Development):
      - Adds the response header **`Strict-Transport-Security`** (HSTS). Default `max-age` is **30 days** if you don’t configure it (e.g., `Strict-Transport-Security: max-age=2592000`). Browsers that honor HSTS will automatically prefer HTTPS for this host going forward.
      - Kept off in Development because browsers cache HSTS aggressively (can “brick” local HTTP testing); loopback hosts are excluded by default.
    - `app.UseHttpsRedirection()`:
      - For an **HTTP** request, returns a redirect using the default **307 Temporary Redirect** and includes a **`Location`** header pointing to the HTTPS URL. This reduces accidental plaintext access (but ideally APIs should not listen on HTTP at all in production).
    - `app.MapGet("/health", () => Results.Ok("ok"))`:
      - Exposes a minimal **health endpoint** for monitors/orchestrators (returns 200 with a tiny body). Content-Type depends on how you return data (e.g., returning a plain string directly is `text/plain`; object results are typically `application/json`). Keep it intentionally non-verbose to avoid leaking internal details.


## 2026-01-31 — HSTS preload documentation

### Added
- `docs/security/hsts-preload.md`
  - **Why**: Documents HSTS preload as an explicit security/ops policy decision (not a default toggle).
  - **Security impact**: Helps avoid unsafe or irreversible rollout (preload requires strict HTTPS on all subdomains and is hard to undo quickly).

- `packages.lock.json` (per project) + CI locked restore
  - **Security impact**: prevents unreviewed transitive dependency drift; CI fails if dependency resolution would change, forcing explicit review of dependency updates.

## 2026-01-31 — NuGet lock file documentation

### Added
- `docs/security/packages.lock.md`
  - **Why**: Documents what `packages.lock.json` locks and how we enforce it in CI.
  - **Security impact**: Prevents unreviewed transitive dependency drift and adds integrity checking via package content hashes.

- NuGet lock files (`packages.lock.json`) + CI locked restore
  - Security impact: prevents silent transitive dependency drift; CI fails unless dependency changes are explicit and reviewable.

## 2026-01-31 — Security policy

### Added
- `SECURITY.md`
  - **Why**: Provides a clear, private path for vulnerability disclosure.
  - **Security impact**: Reduces accidental public exposure of exploit details and standardizes how security issues are handled.

"  - **Why**: Pin GitHub Actions to immutable commit SHAs.\n"
"  - **Security impact**: Prevents a compromised/moved tag (e.g., `@v4`) from silently changing the CI code that executes.\n"


`.github/workflows/*.yml`
"  - **Why**: Pin GitHub Actions to immutable commit SHAs.\n"
"  - **Security impact**: Prevents a compromised/moved tag (e.g., `@v4`) from silently changing the CI code that executes.\n"

## 2026-02-02 — GitHub Actions supply-chain hardening documentation

### Added
- `docs/security/github_action.md`
  - **Why**: Explains why we pin GitHub Actions to full commit SHAs.
  - **Security impact**: Prevents CI code from changing silently via moved/compromised tags and makes workflow code immutable and reviewable. This is not theoretical: in March 2025, a popular third-party GitHub Action was compromised and version tags were retroactively moved to malicious commits (CVE-2025-30066), leading to CI secret exposure.

- Security: Add a PR gate that fails if any workflow uses non-SHA-pinned GitHub Actions, preventing mutable tag references from re-entering the repo (CI supply-chain hardening).

## 2026-02-03 — Remove Server header

### Changed
- `src/ST.Security.Api/Program.cs`
  - **Why**: Disables Kestrel `Server` header (`AddServerHeader=false`).
  - **Security impact**: Reduces HTTP fingerprinting surface. See `docs/security/disable_kestrel_server_header.md`.
## Unreleased
- `Directory.Build.props`
  - **Why**: Pins analyzer baseline to `.NET 10` and enables all Security analyzers.
  - **Security impact**: Adds a code-level security net (CA security rules), not just dependency scanning; prevents silent analyzer drift after SDK updates.
- `.github/workflows/ci.yml`
  - **Why**: Sets `CodeAnalysisTreatWarningsAsErrors=true` in CI builds.
  - **Security impact**: Analyzer findings become a hard PR gate (build fails), so insecure patterns are blocked before merge.
- `docs/security/dotnet_analyzers.md`
  - **Why**: Documents the policy and the tradeoffs for reviewers.

- Dependency vulnerability report (CI artifact)
  - Why: Produce a JSON report of vulnerable dependencies (including transitive) and upload it from CI.
  - Security impact: Easier triage/evidence than log-only NU190x warnings; complements the restore gate. See `docs/security/dependency_vulnerability_report.md`.

- Secret scanning gate (Gitleaks)
  - Security impact: blocks PRs when hardcoded secrets are detected; complements GitHub secret scanning/push protection. See `docs/security/secret_scanning_gitleaks.md`.

## Unreleased

- `.github/workflows/scorecard.yml`
  - **Why**: Runs OpenSSF Scorecard and uploads findings as SARIF to GitHub Code Scanning.
  - **Security impact**: Adds an automated “security posture” gate/visibility layer (repo + CI supply-chain best practices) on top of package vuln scanning and SAST.
  - **Implementation**: SHA-pinned actions + least-privilege permissions (`security-events: write` only for SARIF upload).

- Kestrel DoS hardening (timeouts + size limits)
  - Security impact: reduces slow-header attack window and caps header/body abuse. See `docs/security/kestrel_limits.md`.

- CODEOWNERS for sensitive paths
  - Security impact: forces explicit review ownership for workflow/supply-chain/security policy files; can be enforced via branch protection “Require review from Code Owners”. See `docs/security/codeowners.md`.
