using MSM.Common.Extensions;

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

    public static string GetAllowedOrigin() {
        return Config.GetRequiredValue<string>("AllowedOrigin");
    }

    public static string GetMongoDbUrl() {
        return Config.GetRequiredSection("Mongo").GetRequiredValue<string>("Url");
    }

    public static string GetDiscordToken() {
        return GetDiscordSection().GetRequiredValue<string>("Token");
    }

    public static ulong GetDiscordPxAlertChannelId() {
        return GetDiscordSection().GetRequiredSection("Channels").GetRequiredValue<ulong>("PxAlert");
    }

    public static string GetApiToken() {
        return Config.GetRequiredSection("Api").GetRequiredValue<string>("Token");
    }

    public static int GetAlertIntervalSec() {
        return Config.GetRequiredSection("Alert").GetRequiredValue<int>("Interval");
    }
}