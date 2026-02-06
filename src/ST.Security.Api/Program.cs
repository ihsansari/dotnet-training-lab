using ST.Security.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Hardening: reduce header-based fingerprinting (Kestrel adds `Server` by default)
builder.WebHost.ConfigureKestrel((context, o) =>
{
    o.AddServerHeader = false;

    var cfg = context.Configuration.GetSection("Security:KestrelLimits"); // ":" = nesting in config :contentReference[oaicite:0]{index=0}

    var timeoutSeconds = cfg.GetValue<int?>("RequestHeadersTimeoutSeconds");
    if (timeoutSeconds is > 0)
        o.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(timeoutSeconds.Value); // default 30s :contentReference[oaicite:1]{index=1}

    var maxHeadersBytes = cfg.GetValue<int?>("MaxRequestHeadersTotalSizeBytes");
    if (maxHeadersBytes is > 0)
        o.Limits.MaxRequestHeadersTotalSize = maxHeadersBytes.Value; // limits config supported :contentReference[oaicite:2]{index=2}

    var maxBodyMb = cfg.GetValue<int?>("MaxRequestBodySizeMb");
    if (maxBodyMb is > 0)
        o.Limits.MaxRequestBodySize = maxBodyMb.Value * 1024L * 1024L; // limits config supported :contentReference[oaicite:3]{index=3}
});

var app = builder.Build();

builder.Configuration.WarnIfMissing(
    app.Logger,
    "Security:KestrelLimits",
    "RequestHeadersTimeoutSeconds",
    "MaxRequestHeadersTotalSizeBytes",
    "MaxRequestBodySizeMb"
);

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseHttpsRedirection();

// Baseline security headers (safe defaults for an API)
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
    ctx.Response.Headers.TryAdd("X-Frame-Options", "DENY");
    ctx.Response.Headers.TryAdd("Referrer-Policy", "no-referrer");
    ctx.Response.Headers.TryAdd("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

    await next();
});

app.MapGet("/health", () => Results.Ok("ok"));

app.Run();

internal static partial class Log
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Warning,
        Message = "Missing config '{Key}'. Using defaults/baseline. Recommended: {Recommended}.")]
    internal static partial void MissingConfig(ILogger logger, string key, string recommended);
};