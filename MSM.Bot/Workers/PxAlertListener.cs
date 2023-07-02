using Discord.WebSocket;
using MongoDB.Driver;
using MSM.Bot.Extensions;
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

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
        var channel = await _client.GetPxAlertChannelAsync();
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

            // If alert not found, interval not passed, or already alerted at the same price,
            // don't send message notification
            if (alert is null || alert.AlertedAt == change.FullDocument.Px) {
                return;
            }

            await channel.SendMessageAsync(
                $"Price of **{change.FullDocument.Item}** at **{change.FullDocument.Px:#,###}** now!\n" +
                $"> Alert Threshold: **{alert.MaxPx:#,###}**\n" +
                $"> Last Updated: {change.FullDocument.LastUpdate} (UTC)"
            );
        }, cancellationToken);
    }
}