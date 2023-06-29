using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MSM.Bot.Handlers;
using MSM.Bot.Workers;

var socketConfig = new DiscordSocketConfig {
    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildEmojis,
    AlwaysDownloadUsers = true
};

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => {
        services
            .AddSingleton(socketConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()
            .AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
