# CODEOWNERS for security-critical files

## What this adds
A `.github/CODEOWNERS` file that marks CI workflows and supply-chain config as owned, so review is automatically requested.

## Why it matters (security)
Workflows and dependency configuration directly control what code CI executes and what dependencies get restored.
CODEOWNERS helps prevent accidental or unnoticed changes to those high-impact files.

## How to enforce (GitHub setting)
Enable branch protection on `develop` and turn on “Require review from Code Owners”.
This turns ownership into a hard merge gate (not just a suggestion).
