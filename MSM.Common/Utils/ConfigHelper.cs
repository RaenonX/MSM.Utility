using Microsoft.Extensions.Configuration;

namespace MSM.Common.Utils;

public static class ConfigHelper {
    private static IConfiguration? _config;
    
    private static IConfiguration Config => _config ?? throw new InvalidOperationException("Config not initialized");

    public static void Initialize(IConfiguration? configuration) {
        _config = configuration;
    }

    private static IConfigurationSection GetDiscordSection() {
        return Config.GetRequiredSection("Discord");
    }
    
    public static string GetMongoDbUrl() {
        return Config.GetRequiredSection("Mongo").GetValue<string>("Url") ??
               throw new InvalidOperationException("`Mongo.Url` is required in appsettings.json");
    }

    public static string GetDiscordToken() {
        return GetDiscordSection().GetValue<string>("Token") ??
               throw new InvalidOperationException("`Discord.Token` is required in appsettings.json");
    }

    public static ulong GetDiscordPxAlertChannelId() {
        return GetDiscordSection().GetRequiredSection("Channels").GetValue<ulong>("PxAlert");
    }

    public static string GetApiToken() {
        return Config.GetRequiredSection("Api").GetValue<string>("Token") ??
               throw new InvalidOperationException("`Api.Token` is required in appsettings.json");
    }

    public static int GetAlertIntervalSec() {
        return Config.GetRequiredSection("Alert").GetValue<int>("Interval");
    }
}