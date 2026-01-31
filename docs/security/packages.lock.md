# packages.lock.json (NuGet lock files) — why we use it

## What it locks (exactly)
`packages.lock.json` locks the **resolved dependency closure** for each project:
- **Direct + transitive** NuGet packages (the full graph NuGet resolves)
- The **exact resolved version** per package (per target framework)
- A **content hash** for each package, so restore can detect “same ID+version but different bits” and fail

This is why it’s a supply-chain control: your builds stop drifting silently when transitive dependencies move over time.

## Why it matters (security)
- Prevents **unreviewed transitive drift**: CI fails if dependency resolution changes without an explicit lock file update.
- Makes dependency changes **visible in PR diffs** (reviewable).
- Adds **integrity checking** via content hashes (detects unexpected package content mismatches).

## How we enable it (repo-wide)
We enable lock files globally with:
- `RestorePackagesWithLockFile=true` (in `Directory.Build.props`)

Alternative bootstrap (one-time):
- `dotnet restore --use-lock-file`

## Our CI rule (hard gate)
CI runs restore in locked mode:
- `dotnet restore ... --locked-mode`

Locked mode means: restore must match the lock file, or it fails.

Typical failure:
- `NU1004` when the lock file is inconsistent with project dependencies (e.g., changed PackageReference versions, changed TargetFrameworks, etc.).

## Developer workflow
### Update dependencies (intentional change)
1) Modify `Directory.Packages.props` / `.csproj`
2) Run restore to update lock files:
   - `dotnet restore --use-lock-file` (or just `dotnet restore` when lock is enabled)
3) Commit:
   - `.csproj` / `Directory.Packages.props`
   - `packages.lock.json` files

### Keep CI deterministic
CI uses:
- `dotnet restore --locked-mode`
- `dotnet build --no-restore ...`

So restore is the single dependency policy checkpoint.

## Concrete example
Project references a direct package:
```xml
<ItemGroup>
  <PackageReference Include="Example.Package" Version="1.2.3" />
</ItemGroup>
Lock file records exact resolution + integrity:

{
  "version": 1,
  "dependencies": {
    "net10.0": {
      "Example.Package": {
        "type": "Direct",
        "requested": "[1.2.3, )",
        "resolved": "1.2.3",
        "contentHash": "sha512-..."
      },
      "Some.Transitive.Dependency": {
        "type": "Transitive",
        "resolved": "4.5.6",
        "contentHash": "sha512-..."
      }
    }
  }
}
What this prevents (in plain terms)
Without lock files, NuGet’s dependency resolution rules (including floating versions and transitive updates) can produce a different graph across time/machines.
With lock files + locked mode, dependency changes must be explicit (PR diffs + review), or CI fails.

