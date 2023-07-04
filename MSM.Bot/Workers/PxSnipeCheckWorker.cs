using Discord.WebSocket;
using MSM.Bot.Extensions;
using MSM.Bot.Utils;
using MSM.Common.Controllers;

namespace MSM.Bot.Workers;

public class PxSnipeCheckWorker : BackgroundService {
    private readonly DiscordSocketClient _client;

    private readonly ILogger<PxSnipeCheckWorker> _logger;

    private bool _failed;

    internal static readonly TimeSpan LastValidTickMaxGap = TimeSpan.FromSeconds(3);

    internal static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(2);

    public PxSnipeCheckWorker(DiscordSocketClient client, ILogger<PxSnipeCheckWorker> logger) {
        _client = client;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
        var channel = await _client.GetSnipingAlertChannelAsync();

        while (!cancellationToken.IsCancellationRequested) {
            await Task.Delay(CheckInterval, cancellationToken);

            var sniping = await PxSnipingItemController.GetSnipingItemAsync();

            if (sniping is null) {
                continue;
            }

            var lastTickTimestamp = await PxMetaController.GetLastValidTickUpdate(sniping.Item);

            if (lastTickTimestamp is null) {
                // No valid tick
                _logger.LogWarning("Item on snipe ({Item}) does not have any valid tick!", sniping.Item);
                await channel.SendMessageAsync(
                    $"Item on snipe (**{sniping.Item}**) does not have any valid tick!",
                    embed: DiscordMessageMaker.MakeCurrentSnipingNoUpdate(sniping, lastTickTimestamp)
                );
                _failed = true;
            } else if (DateTime.UtcNow - lastTickTimestamp > LastValidTickMaxGap) {
                // No valid tick within certain time
                _logger.LogWarning(
                    "Item on snipe ({Item}) failed valid tick check (Last tick at {LastValidTickUpdate})",
                    sniping.Item,
                    lastTickTimestamp
                );
                await channel.SendMessageAsync(
                    $"No sniping price update of **{sniping.Item}**!",
                    embed: DiscordMessageMaker.MakeCurrentSnipingNoUpdate(sniping, lastTickTimestamp)
                );
                _failed = true;
            } else {
                // Found valid tick
                if (_failed) {
                    await channel.SendMessageAsync(
                        $"Sniping price alert of **{sniping.Item}** start ticking again!",
                        embed: await DiscordMessageMaker.MakeCurrentSnipingInfo(sniping)
                    );
                }

                _logger.LogInformation(
                    "Tick check of the item on snipe ({Item}) succeed (Last tick at {LastValidTickUpdate})",
                    sniping.Item,
                    lastTickTimestamp
                );
                _failed = false;
            }
        }
    }
}