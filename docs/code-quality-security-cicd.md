# Code Quality / Security / CI-CD Baseline (root-level)

This document describes the **root baseline** that makes the repository safer and more predictable before adding application code.

## What this baseline protects
- **Reproducible builds**: same SDK/toolchain locally and in CI.
- **Supply-chain security**: detect vulnerable NuGet dependencies early and block merges.
- **Maintainability**: central place for repo-wide build rules and package versions.

---

## Root files and their role

### `global.json`
**Role (workflow / code quality):**
- Pins the .NET **SDK** version used by `dotnet` commands across machines and CI.

**Role (security):**
- Reduces toolchain drift and helps ensure the repo consistently uses patched SDK builds (less “it builds on my machine”).  
- SDK selection is **independent from the runtime TFM** you will target in projects (net8/net10 later).  

---

### `Directory.Build.props`
**Role (workflow / code quality):**
- Central place for repo-wide MSBuild defaults (applies automatically to projects under the folder tree).

**Role (security):**
- Enables **NuGet package auditing** during restore (direct + transitive depending on settings/TFM).
- Defines the policy threshold (ex: audit level = moderate) and escalates vulnerability warnings to errors in CI.

---

### `Directory.Packages.props`
**Role (workflow / code quality):**
- Turns on **Central Package Management (CPM)** so package versions live in one place.

**Role (security):**
- Makes patching dependency versions faster and consistent (less “version drift” across projects).

---

### `.github/workflows/ci.yml`
**Role (workflow / CI/CD):**
- Runs restore + build for PRs / pushes to `develop` using the SDK pinned in `global.json`.

**Role (security):**
- Turns known vulnerable dependencies into a **hard CI gate** (job fails => PR cannot merge when checks are required).

**Enforced commands (security gate):**
- `dotnet restore ST.Security.slnx -warnaserror:NU1902,NU1903,NU1904`
- `dotnet build ST.Security.slnx --no-restore -p:ContinuousIntegrationBuild=true`

**Why this matters (CTO-level):**
- On .NET 8+ SDKs, restore emits vulnerability warnings by default, and:
  - `NU1902` = moderate
  - `NU1903` = high
  - `NU1904` = critical  
  Treating them as errors prevents merging code that introduces known-vulnerable dependencies.
- `--no-restore` ensures the build step doesn’t do an implicit restore; restore stays the single enforced policy checkpoint.

---

### `.github/dependabot.yml`
**Role (workflow / CI/CD):**
- Creates automated PRs to update NuGet dependencies and GitHub Actions.

**Role (security):**
- Reduces time-to-patch for supply-chain issues by continuously proposing safe upgrades.

---

### `CHANGELOG.md`
**Role (workflow):**
- Auditable “why we did this” record for security and engineering decisions.

**Role (security):**
- Makes the security intent explicit for reviewers and future maintainers.

---

## Notes for .NET 10 (later)
When you introduce projects targeting `net10.0`, `dotnet restore` can **audit transitive dependencies by default** for those projects, increasing coverage (and potentially increasing findings). Plan to handle that with dependency upgrades and/or a clear policy threshold.

