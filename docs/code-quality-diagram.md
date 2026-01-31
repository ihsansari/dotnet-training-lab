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