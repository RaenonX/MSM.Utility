using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MUB.Main.Handlers;
using MUB.Main.Workers;

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
