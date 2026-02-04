var builder = WebApplication.CreateBuilder(args);

// Hardening: reduce header-based fingerprinting (Kestrel adds `Server` by default)
builder.WebHost.ConfigureKestrel((context, o) =>
{
    // removes "Server: Kestrel"
    o.AddServerHeader = false;

    builder.WebHost.ConfigureKestrel(o =>
    {
        o.AddServerHeader = false;
        o.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(15);
        o.Limits.MaxRequestHeadersTotalSize = 16 * 1024;
        o.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
    });
});

var app = builder.Build();

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
