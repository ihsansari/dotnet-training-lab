# Secret scanning in CI (Gitleaks)

## What this adds
A CI job that fails PRs/pushes to `develop` if a hardcoded secret is detected in the repo (current changes and history).

## Why it matters
GitHub secret scanning + push protection is a strong baseline for public repos, but it primarily targets known secret formats.
Gitleaks adds an extra layer (heuristics + broad detectors) and gives you a CI gate that fails fast when something slips in.

## Security posture
- Workflow token is least-privilege (`contents: read`).
- Actions are SHA-pinned to avoid mutable tag supply-chain risk.
