﻿using MSM.Common.Controllers;
using MSM.Common.Utils;

namespace MSM.Common.Extensions;

public static class InitializingExtensions {
    public static IHostBuilder BuildCommon(this IHostBuilder builder) {
        builder.ConfigureServices((context, _) => { ConfigHelper.Initialize(context.Configuration); });
        builder.ConfigureLogging(logging => logging.AddSimpleConsole(LogHelper.LoggingConfigureAction));

        return builder;
    }

    public static WebApplicationBuilder BuildCommon(this WebApplicationBuilder builder) {
        ConfigHelper.Initialize(builder.Configuration);
        builder.Services.AddLogging(logging => logging.AddSimpleConsole(LogHelper.LoggingConfigureAction));

        return builder;
    }

    public static async Task BootAsync(this IHost host) {
        await MongoManager.Initialize();
        await host.RunAsync();
    }

    public static T InitLogging<T>(this T host) where T : IHost {
        LogHelper.Factory = host.Services.GetRequiredService<ILoggerFactory>();

        return host;
    }
}