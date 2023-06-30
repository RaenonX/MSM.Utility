using Discord;
using Discord.WebSocket;
using MongoDB.Driver;
using MSM.Common.Controllers;
using MSM.Common.Models;
using MSM.Common.Utils;

namespace MSM.Bot.Workers;

public class PxAlertListener : BackgroundService {
    private readonly DiscordSocketClient _client;

    public PxAlertListener(DiscordSocketClient client) {
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
        var pxAlertChannelId = ConfigHelper.GetDiscordPxAlertChannelId();
        if (await _client.GetChannelAsync(pxAlertChannelId) is not IMessageChannel channel) {
            throw new ArgumentException($"Px alert channel is not a message channel (#{pxAlertChannelId})");
        }

        var options = new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup };
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<PxMetaModel>>()
            .Match(x => x.OperationType == ChangeStreamOperationType.Insert);

        using var cursor = await MongoConst
            .PxMetaCollection
            .WatchAsync(pipeline, options, cancellationToken);

        await cursor.ForEachAsync(async change => {
            var alert = await PxAlertController.GetAlert(change.FullDocument.Item, change.FullDocument.Px);

            // Alert not updated - shouldn't send message notification
            if (alert is null) {
                return;
            }

            await channel.SendMessageAsync(
                $"Price of **{change.FullDocument.Item}** dropped below **{alert.MaxPx:#,###}**!\n" +
                $"> Current Px: **{change.FullDocument.Px:#,###}**\n" +
                $"> Last Updated: {change.FullDocument.LastUpdate} (UTC)"
            );
        }, cancellationToken);
    }
}