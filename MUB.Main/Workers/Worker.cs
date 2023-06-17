using Discord;
using Discord.WebSocket;
using MUB.Main.Handlers;

namespace MUB.Main.Workers;

public class Worker : BackgroundService {
    private readonly IConfiguration _config;

    private readonly IServiceProvider _services;

    public Worker(IServiceProvider services, IConfiguration config) {
        _services = services;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
        var client = _services.GetRequiredService<DiscordSocketClient>();

        client.Log += OnLogHandler.OnLogAsync;

        // Initializes service and register command
        await _services
            .GetRequiredService<InteractionHandler>()
            .InitializeAsync();

        // Bot token can be provided from the Configuration object we set up earlier
        await client.LoginAsync(TokenType.Bot, _config.GetSection("Discord").GetValue<string>("Token"));
        await client.StartAsync();

        // Never quit the program until manually forced to.
        await Task.Delay(Timeout.Infinite, cancellationToken);
    }
}