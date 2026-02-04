# .NET analyzers in CI (security)

## What this adds
We enable .NET SDK analyzers (CA rules) with an explicit baseline, and we fail CI if analyzer warnings occur.

## Why it matters (security)
Dependency scanning catches known vulnerable packages.
Analyzers catch insecure *code patterns* (misuse of crypto/TLS, risky APIs, etc.) during build, before merge.

## Why we pin the analyzer baseline
We pin `AnalysisLevel` to a .NET 10 baseline to avoid surprise new warnings after an SDK update.
You can upgrade the baseline intentionally when you want.

## References (MSBuild knobs)
- AnalysisLevel / AnalysisMode / AnalysisModeSecurity
- CodeAnalysisTreatWarningsAsErrors


## Examples: what CI will block

### 1) TLS certificate validation bypass (CA5359)
**Before (blocked):**
```csharp
var handler = new HttpClientHandler
{
    // Accepts ANY cert => trivial MITM on internal networks / proxies
    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
};
using var http = new HttpClient(handler);
```

Why it matters: disabling validation turns HTTPS into “encryption without authentication”, enabling man-in-the-middle attacks. 

2) Insecure randomness for tokens/keys (CA5394)

```csharp
var token = new Random().Next().ToString(); // predictable => guessable tokens
```

Why it matters: System.Random is not cryptographically secure; attackers can predict outputs in security-sensitive contexts. 


3) Insecure deserialization (BinaryFormatter) (CA2300)

```csharp

var bf = new BinaryFormatter();
var obj = (MyType)bf.Deserialize(stream);
```

Why it matters: insecure deserializers can allow type injection and lead to remote code execution or data compromise when input is attacker-controlled. 