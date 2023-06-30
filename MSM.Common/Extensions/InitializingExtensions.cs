using MSM.Common.Controllers;
using MSM.Common.Utils;

namespace MSM.Common.Extensions; 

public static class InitializingExtensions {
    public static IHostBuilder InitConfig(this IHostBuilder builder) {
        builder.ConfigureServices((context, _) => {
            ConfigHelper.Initialize(context.Configuration);
        });

        return builder;
    }

    public static WebApplicationBuilder InitConfig(this WebApplicationBuilder builder) {
        ConfigHelper.Initialize(builder.Configuration);
        
        return builder;
    }

    public static async Task BootAsync(this IHost host) {
        await MongoManager.Initialize();
        await host.RunAsync();
    }
}