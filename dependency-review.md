# dependency-review vs ci.yml (security)

## What ci.yml already enforces (hard gate)
- `dotnet restore` runs with NuGet auditing enabled and emits vulnerability warnings by default on .NET 8+ SDKs.
- In CI we treat NU1902–NU1904 as errors, so moderate/high/critical vulnerable dependencies (direct or transitive) fail the job and block merges.

## What dependency-review adds (extra value)
- PR-diff focused: it reviews what the pull request is introducing (dependency changes), so reviewers immediately see “this PR added X@Y and it is vulnerable”.
- It can fail PRs based on severity (e.g., `fail-on-severity: moderate`) and can also enforce policy such as allowed/denied licenses.
- It complements ci.yml: dependency-review improves PR visibility and policy controls, while ci.yml remains the authoritative restore/build gate on the actual resolved graph.

## Notes
- dependency-review requires GitHub Dependency Graph to be enabled for the repository.
- If you target .NET 10 (`net10.0`), NuGet auditing includes transitive packages by default, which makes the restore gate stronger as the repo grows.
