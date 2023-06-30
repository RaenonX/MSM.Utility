using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MSM.Bot.Handlers;
using MSM.Bot.Workers;
using MSM.Common.Extensions;

var socketConfig = new DiscordSocketConfig {
    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildEmojis,
    AlwaysDownloadUsers = true
};

var host = Host.CreateDefaultBuilder(args)
    .InitConfig()
    .ConfigureServices(services => {
        services
            .AddSingleton(socketConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()
            .AddHostedService<DiscordClientWorker>()
            .AddHostedService<PxAlertListener>();
    })
    .Build();

await host.BootAsync();
