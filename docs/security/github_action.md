# GitHub Actions hardening

## Why we pin actions to full commit SHAs

GitHub Actions referenced by tags (e.g., `@v4`) are *mutable*: if an upstream repo is compromised, an attacker can move a tag and silently change what your CI executes.
Pinning to a full commit SHA makes the action reference immutable and drastically reduces CI supply-chain risk. :contentReference[oaicite:0]{index=0}

## Why we enforce this with a dedicated workflow

Even if we pin actions today, a future PR could reintroduce `@vX` by mistake.
This workflow fails the PR if any workflow uses non-SHA-pinned actions, keeping the rule permanent and reviewable in CI. :contentReference[oaicite:1]{index=1}

## This is not theoretical (real incident)

In March 2025, the widely used `tj-actions/changed-files` action was compromised and version tags were retroactively moved to malicious commits; hash-pinned users were largely protected.
That’s exactly the class of failure SHA pinning is designed to prevent. :contentReference[oaicite:2]{index=2}
