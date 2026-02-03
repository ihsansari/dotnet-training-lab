var builder = WebApplication.CreateBuilder(args);

// Hardening: reduce header-based fingerprinting (Kestrel adds `Server` by default)
builder.WebHost.ConfigureKestrel(o => o.AddServerHeader = false);

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
