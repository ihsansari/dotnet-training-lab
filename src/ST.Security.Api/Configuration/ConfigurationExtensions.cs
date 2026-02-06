using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ST.Security.Api.Logging;

namespace ST.Security.Api.Configuration;

internal static class ConfigurationExtensions
{
    internal static void WarnIfMissing(
        this IConfiguration cfg,
        ILogger log,
        string sectionPath,
        params string[] keys)
    {
        // "A:B" paths are standard config hierarchy.
        var section = cfg.GetSection(sectionPath); 

        foreach (var key in keys)
        {
            if (section.GetValue<string?>(key) is null)
                LogMessages.MissingConfig(log, $"{sectionPath}:{key}");
        }
    }
}