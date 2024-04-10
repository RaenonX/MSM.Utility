using Discord.WebSocket;
using MSM.Bot.Enums;
using MSM.Bot.Extensions;
using MSM.Bot.Utils;
using MSM.Common.Controllers;

namespace MSM.Bot.Workers;

public class PxUpdateCheckWorker : BackgroundService {
    private readonly DiscordSocketClient _client;

    private readonly ILogger<PxUpdateCheckWorker> _logger;

    private bool _failed;

    private static readonly TimeSpan LastValidTickMaxGap = TimeSpan.FromDays(365);

    private static readonly TimeSpan CheckInterval = TimeSpan.FromDays(365);

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
                await channel.SendMessageAsync(
                    "No last valid tick update found!",
                    embed: DiscordMessageMaker.MakeLastValidTick(lastTickTimestamp, Colors.Danger)
                );
                _failed = true;
                continue;
            }

            if (DateTime.UtcNow - lastTickTimestamp > LastValidTickMaxGap) {
                // No valid tick within certain time
                _logger.LogWarning(
                    "Last valid tick check failed (Last tick at {LastValidTickUpdate})",
                    lastTickTimestamp
                );
                await channel.SendMessageAsync(
                    "Not receiving price updates!",
                    embed: DiscordMessageMaker.MakeLastValidTick(lastTickTimestamp, Colors.Danger)
                );
                _failed = true;
                continue;
            }

            // Found valid tick
            if (_failed) {
                await channel.SendMessageAsync(
                    "Price starts ticking again!",
                    embed: DiscordMessageMaker.MakeLastValidTick(lastTickTimestamp, Colors.Success)
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
