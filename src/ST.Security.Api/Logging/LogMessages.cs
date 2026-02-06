using Microsoft.Extensions.Logging;

namespace ST.Security.Api.Logging;

/// <summary>
/// Centralized, source-generated log messages for this app.
/// - We use <see cref="LoggerMessageAttribute"/> to satisfy CA1848 and avoid the runtime costs of
///   logger extension methods (template parsing, params-array allocations, boxing).
/// - Keep EventIds stable so alerts/dashboards can rely on them.
/// - Placeholders (e.g., {Key}) become structured log properties.
/// </summary>
internal static partial class LogMessages
{
    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Warning,
        Message = "Missing config '{Key}'. Using configured/code defaults.")]
    internal static partial void MissingConfig(ILogger logger, string key);
}

