```mermaid
flowchart LR
  Dev["Developer"] --> PR["PR to develop"]
  PR --> CI["GitHub Actions<br/>ci.yml"]

  CI --> SDK["setup-dotnet<br/>uses global.json"]
  SDK --> R["dotnet restore<br/>-warnaserror:NU1902,NU1903,NU1904"]

  R -->|vulnerabilities found| Fail["CI fails<br/>merge blocked"]
  R -->|clean| B["dotnet build --no-restore<br/>-p:ContinuousIntegrationBuild=true"]

  B --> Pass["CI passes"]
  Pass --> Merge["Merge to develop"]
flowchart TB
  GJ["global.json<br/>pin SDK"] --> Toolchain["Reproducible toolchain"]

  DPP["Directory.Packages.props<br/>central versions (CPM)"] --> Versions["Controlled package versions"]
  DBP["Directory.Build.props<br/>NuGet audit policy"] --> Audit["Detect known vulnerable dependencies"]

  DEP["dependabot.yml<br/>automated updates"] --> Updates["Update PRs"]
  CIY["ci.yml<br/>enforced CI gate"] --> Gate["Blocks vulnerable dependencies"]
  CL["CHANGELOG.md<br/>security rationale"] --> Rationale["Audit trail"]

  Toolchain --> Gate
  Versions --> Audit
  Updates --> Versions
  Audit --> Gate
  Rationale --> Gate
flowchart LR
  Change["Dependency change<br/>(direct or transitive)"] --> Restore["Restore + audit"]
  Restore -->|NU1902-NU1904| Stop["Fail CI"]
  Restore -->|no findings| Continue["Proceed (build/review/merge)"]
