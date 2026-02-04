# OpenSSF Scorecard (repo security posture)

## What this adds
This repo runs OpenSSF Scorecard in CI and uploads the results to GitHub Code Scanning (SARIF).

## Why it matters (security)
Scorecard is a **supply-chain / repo-hardening** signal: it checks common weaknesses like weak branch protection, risky workflow patterns, missing update hygiene, and other security best-practice gaps. It’s a cheap safety-net to catch “process” regressions early.

## Where to see the report
- GitHub → **Security** → **Code scanning alerts**
  - Filter by tool: **Scorecard**
- You also get a build artifact named `scorecard-results` containing `results.sarif`.

## Notes
- The scheduled run executes on the repository default branch. If `develop` is not the default branch, keep `push: develop` (still works) and consider setting `develop` as default if you want the weekly schedule to run there.
