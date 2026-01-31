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

- `dotnet restore -warnaserror NU1902;NU1903;NU1904`
- `dotnet build --no-restore -p:ContinuousIntegrationBuild=true`

**What we gain:** `dotnet restore` emits vulnerability warnings by default on .NET 8+ SDKs, and `NU1902–NU1904` correspond to **moderate / high / critical** known vulnerabilities. Turning them into errors makes vulnerable dependencies a **hard CI gate** (the job fails, so the PR cannot be merged).

`--no-restore` ensures the build step does not perform an implicit restore (which could otherwise re-run dependency resolution) and keeps **restore as the single enforced policy point**. Setting `ContinuousIntegrationBuild=true` enables CI-only build behavior recommended for official builds and lets repo rules apply consistently in CI.
