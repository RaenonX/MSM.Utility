using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MSM.Bot.Handlers;
using MSM.Bot.Workers;
using MSM.Common.Controllers;
using MSM.Common.Utils;

var socketConfig = new DiscordSocketConfig {
    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildEmojis,
    AlwaysDownloadUsers = true
};

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) => {
        ConfigHelper.Initialize(context.Configuration);
        
        services
            .AddSingleton(socketConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()
            .AddHostedService<Worker>();
    })
    .Build();

await MongoManager.Initialize();

await host.RunAsync();
