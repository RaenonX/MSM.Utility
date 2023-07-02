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

        await _services.GetRequiredService<InteractionHandler>().InitializeAsync();

        await _client.LoginAsync(TokenType.Bot, ConfigHelper.GetDiscordToken());
        await _client.StartAsync();

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }
}