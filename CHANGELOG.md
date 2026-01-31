# Changelog

This repository is a training lab, but the baseline is **security-first**:
reproducible builds + supply-chain hygiene (fast patching of known vulnerabilities).

## 2026-01-31 — Root security baseline

### Added

- `global.json`
  - **Why**: Pins the .NET SDK used by developers and CI.
  - **Security impact**: Reduces “works on my machine” drift and makes it easier to roll out patched SDKs consistently (same toolchain everywhere). :contentReference[oaicite:0]{index=0}

- `Directory.Build.props`
  - **Why**: Central place for repo-wide MSBuild rules (applies automatically to projects under the folder).
  - **Security impact**: Enables **NuGet auditing** and (in CI) escalates vulnerability warnings (NU1902–NU1904) to errors so vulnerable restores can’t silently ship. :contentReference[oaicite:1]{index=1}

- `Directory.Packages.props`
  - **Why**: Central Package Management (CPM) keeps all package versions in one file.
  - **Security impact**: Faster and safer upgrades (one place to patch versions), reduces dependency drift across projects. :contentReference[oaicite:2]{index=2}

- `.github/dependabot.yml`
  - **Why**: Automates dependency update PRs (NuGet + GitHub Actions).
  - **Security impact**: Shortens the time-to-patch for supply-chain issues by continuously proposing updates. Note: security updates are tied to the default branch behavior, so keep branch strategy in mind. :contentReference[oaicite:3]{index=3}

- `ST.Security.sln`
  - **Why**: Stable entry point for restore/build across tools and CI.
  - **Security impact**: Makes it easy to run consistent restore/audit/build checks as the repo grows (single command surface). :contentReference[oaicite:4]{index=4}
