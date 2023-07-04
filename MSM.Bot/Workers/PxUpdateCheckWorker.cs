using Discord.WebSocket;
using MSM.Bot.Extensions;
using MSM.Common.Controllers;

namespace MSM.Bot.Workers;

public class PxUpdateCheckWorker : BackgroundService {
    private readonly DiscordSocketClient _client;

    private readonly ILogger<PxUpdateCheckWorker> _logger;

    private bool _failed;

    private static readonly TimeSpan LastValidTickMaxGap = TimeSpan.FromSeconds(45);

    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(30);

    public PxUpdateCheckWorker(DiscordSocketClient client, ILogger<PxUpdateCheckWorker> logger) {
        _client = client;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
        var channel = await _client.GetSystemAlertChannelAsync();

        while (!cancellationToken.IsCancellationRequested) {
            await Task.Delay(CheckInterval, cancellationToken);

            var lastTickTimestamp = await PxMetaController.GetLastValidTickUpdate();

            if (lastTickTimestamp is null) {
                // No valid tick
                _logger.LogWarning("Last valid tick check failed (No valid tick)");
                await channel.SendMessageAsync("No last valid tick update found!");
                _failed = true;
                continue;
            }

            if (DateTime.UtcNow - lastTickTimestamp > LastValidTickMaxGap) {
                // No valid tick within certain time
                var secsAgo = (DateTime.UtcNow - lastTickTimestamp.Value).TotalSeconds;

                _logger.LogWarning(
                    "Last valid tick check failed (Last tick at {LastValidTickUpdate})",
                    lastTickTimestamp
                );
                await channel.SendMessageAsync(
                    $"No price update since **{secsAgo:0} secs ago**!\n" +
                    $"> Last valid tick updated at {lastTickTimestamp} (UTC)"
                );
                _failed = true;
                continue;
            }

            // Found valid tick
            if (_failed) {
                await channel.SendMessageAsync(
                    $"Prices start ticking again!\n" +
                    $"> Received price update at **{lastTickTimestamp}** (UTC)"
                );
            }

            _logger.LogInformation(
                "Last valid tick check succeed (Last tick at {LastValidTickUpdate})",
                lastTickTimestamp
            );
            _failed = false;
        }
    }
}