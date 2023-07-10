using Discord;
using Discord.WebSocket;
using MongoDB.Driver;
using MSM.Bot.Extensions;
using MSM.Bot.Utils;
using MSM.Common.Controllers;
using MSM.Common.Models;

namespace MSM.Bot.Workers;

public class PxAlertListener : BackgroundService {
    private readonly DiscordSocketClient _client;

    private readonly ILogger<PxAlertListener> _logger;

    public PxAlertListener(DiscordSocketClient client, ILogger<PxAlertListener> logger) {
        _client = client;
        _logger = logger;
    }

    private async Task CheckRegularPxAlert(IMessageChannel channel, PxMetaModel updatedMeta) {
        var triggeredAlerts = await PxAlertController.GetAlert(updatedMeta.Item, updatedMeta.Px);

        if (triggeredAlerts.Count == 0) {
            return;
        }

        foreach (var alert in triggeredAlerts) {
            _logger.LogInformation(
                "Triggered general Px alert on {Item} at {Px} (< {PxAlert}) for UID {AlertUser}",
                updatedMeta.Item,
                updatedMeta.Px,
                alert.MaxPx,
                alert.UserId
            );
        }

        await channel.SendMessageAsync(
            $"Price of **{updatedMeta.Item}** at {updatedMeta.Px.ToMesoText()} now! " +
            $"{string.Join(' ', triggeredAlerts.Select(x => MentionUtils.MentionUser(x.UserId)))}",
            embeds: triggeredAlerts.Select(x => DiscordMessageMaker.MakePriceAlert(updatedMeta, x)).ToArray()
        );
    }

    private async Task CheckSnipingPxAlert(IMessageChannel channel, PxMetaModel updatedMeta) {
        var sniping = await PxSnipingItemController.GetSnipingItemAsync();

        if (sniping is null || updatedMeta.Item != sniping.Item || updatedMeta.Px > sniping.Px) {
            // Not sniping / Updated meta not on snipe / Price not under threshold
            return;
        }

        _logger.LogInformation(
            "Triggered sniping Px alert on {Item} for {Px} (< {PxAlert})",
            updatedMeta.Item,
            updatedMeta.Px,
            sniping.Px
        );

        await channel.SendMessageAsync(
            $"**{sniping.Item}** ready to snipe!",
            embed: DiscordMessageMaker.MakeCurrentSnipingInfo(sniping, updatedMeta)
        );
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
        var pxAlertChannel = await _client.GetPxAlertChannelAsync();
        var snipingAlertChannel = await _client.GetSnipingAlertChannelAsync();

        var options = new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup };
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<PxMetaModel>>()
            .Match(x =>
                x.OperationType == ChangeStreamOperationType.Update ||
                x.OperationType == ChangeStreamOperationType.Modify
            );

        using var cursor = await MongoConst
            .PxMetaCollection
            .WatchAsync(pipeline, options, cancellationToken);

        await cursor.ForEachAsync(async change => {
            var updatedMeta = change.FullDocument;

            _logger.LogInformation(
                "Received Px meta change on {Item} for {Px}",
                updatedMeta.Item,
                updatedMeta.Px
            );

            await Task.WhenAll(
                CheckRegularPxAlert(pxAlertChannel, updatedMeta),
                CheckSnipingPxAlert(snipingAlertChannel, updatedMeta)
            );
        }, cancellationToken);
    }
}