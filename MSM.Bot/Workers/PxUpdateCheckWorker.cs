using Discord.WebSocket;
using MSM.Bot.Extensions;
using MSM.Common.Controllers;

namespace MSM.Bot.Workers;

public class PxUpdateCheckWorker : BackgroundService {
    private readonly DiscordSocketClient _client;

    private readonly ILogger<PxUpdateCheckWorker> _logger;

    public PxUpdateCheckWorker(DiscordSocketClient client, ILogger<PxUpdateCheckWorker> logger) {
        _client = client;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
        var channel = await _client.GetPxAlertChannel();

        while (!cancellationToken.IsCancellationRequested) {
            var lastValidTickUpdate = await PxMetaController.GetLastValidTickUpdate();

            if (lastValidTickUpdate is null) {
                _logger.LogWarning("Last valid tick check failed (No valid tick)");
                await channel.SendMessageAsync("No last valid tick update found!");
            } else if (DateTime.UtcNow - lastValidTickUpdate > TimeSpan.FromSeconds(45)) {
                var secsAgo = (DateTime.UtcNow - lastValidTickUpdate.Value).TotalSeconds;

                _logger.LogWarning(
                    "Last valid tick check failed (Last tick at {LastValidTickUpdate})",
                    lastValidTickUpdate
                );
                await channel.SendMessageAsync(
                    $"No price update since **{secsAgo:0} secs ago**!\n" +
                    $"> Last valid tick updated at {lastValidTickUpdate} (UTC)"
                );
            } else {
                _logger.LogInformation(
                    "Last valid tick check succeed (Last tick at {LastValidTickUpdate})",
                    lastValidTickUpdate
                );
            }
            await Task.Delay(30000, cancellationToken);
        }
    }
}