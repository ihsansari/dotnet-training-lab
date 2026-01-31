```mermaid
flowchart LR
  Dev[Developer] -->|push| PR[Pull Request -> develop]
  PR --> CI[GitHub Actions CI]

  CI --> SDK[setup-dotnet (global.json)]
  SDK --> R[dotnet restore\n-warnaserror: NU1902,NU1903,NU1904]
  R -->|NU1902/3/4 found| Fail[CI fails\nmerge blocked]
  R -->|OK| B[dotnet build --no-restore\n-p:ContinuousIntegrationBuild=true]
  B --> Pass[CI passes]
  Pass --> Merge[Merge to develop]
flowchart TB
  GJ[global.json\npin SDK] --> Toolchain[Reproducible toolchain]
  DBP[Directory.Build.props\nNuGetAudit + CI policy] --> Audit[Detect vulnerable dependencies]
  DPP[Directory.Packages.props\nCPM] --> Versions[Centralized versions]
  DEP[dependabot.yml] --> Updates[Automated update PRs]
  CIY[ci.yml] --> Gate[Enforced CI gate]
  CL[CHANGELOG.md] --> Rationale[Decision trace]

  Toolchain --> Gate
  Audit --> Gate
  Versions --> Audit
  Updates --> Versions
  Rationale --> Gate
flowchart LR
  Change[New package reference\n(direct or transitive)] --> Restore[dotnet restore]
  Restore -->|NU1902/NU1903/NU1904| Stop[Stop: fail CI]
  Restore -->|No NU1902-1904| Continue[Continue: build/test/review]
