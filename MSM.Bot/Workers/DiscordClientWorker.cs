using Discord;
using Discord.WebSocket;
using MSM.Bot.Handlers;
using MSM.Common.Utils;

namespace MSM.Bot.Workers;

public class DiscordClientWorker : BackgroundService {
    private readonly IServiceProvider _services;

    private readonly DiscordSocketClient _client;

    public DiscordClientWorker(IServiceProvider services, DiscordSocketClient client) {
        _services = services;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
        _client.Log += OnLogHandler.OnLogAsync;

        // Initializes service and register command
        await _services
            .GetRequiredService<InteractionHandler>()
            .InitializeAsync();

        // Bot token can be provided from the Configuration object we set up earlier
        await _client.LoginAsync(TokenType.Bot, ConfigHelper.GetDiscordToken());
        await _client.StartAsync();

        // Never quit the program until manually forced to.
        await Task.Delay(Timeout.Infinite, cancellationToken);
    }
}