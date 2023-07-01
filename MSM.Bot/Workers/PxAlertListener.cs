using Discord;
using Discord.WebSocket;
using MongoDB.Driver;
using MSM.Bot.Extensions;
using MSM.Common.Controllers;
using MSM.Common.Models;
using MSM.Common.Utils;

namespace MSM.Bot.Workers;

public class PxAlertListener : BackgroundService {
    private readonly DiscordSocketClient _client;

    private readonly ILogger<PxAlertListener> _logger;

    public PxAlertListener(DiscordSocketClient client, ILogger<PxAlertListener> logger) {
        _client = client;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
        var channel = await _client.GetPxAlertChannel();
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
            var alert = await PxAlertController.GetAlert(change.FullDocument.Item, change.FullDocument.Px);

            _logger.LogInformation(
                "Received Px meta change on {Item} for {Px} ({WillAlert})",
                change.FullDocument.Item,
                change.FullDocument.Px,
                alert is null ? "Non-alert" : "Alert"
            );

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