using Discord;
using Discord.WebSocket;
using MongoDB.Driver;
using MSM.Common.Controllers;
using MSM.Common.Extensions;
using MSM.Common.Models;
using MSM.Common.Utils;

namespace MSM.Bot.Workers;

public class PxAlertListener : BackgroundService {
    private readonly DiscordSocketClient _client;

    public PxAlertListener(DiscordSocketClient client) {
        _client = client;
    }

    private static void WatchPxCollection(
        IMessageChannel channel,
        string item,
        CancellationToken cancellationToken
    ) {
        var options = new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup };
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<PxDataModel>>()
            .Match(x => x.OperationType == ChangeStreamOperationType.Insert);

        var cursor = MongoConst.GetPxTickCollection(item).Watch(pipeline, options, cancellationToken);

        TaskHelper.FireAndForget(
            async () => {
                using var enumerator = cursor.ToEnumerable(cancellationToken: cancellationToken).GetEnumerator();
                while (enumerator.MoveNext()) {
                    var doc = enumerator.Current;

                    if (doc is null) {
                        continue;
                    }

                    var alert = await PxAlertController.GetAlert(item, doc.FullDocument.Px);

                    // Alert not updated - shouldn't send message notification
                    if (alert is null) {
                        continue;
                    }

                    var secsAgo = (DateTime.UtcNow - doc.FullDocument.Timestamp).TotalSeconds;

                    await channel.SendMessageAsync(
                        $"Price of **{item}** dropped below **{alert.MaxPx:#,###}**!\n" +
                        $"> Current Px: **{doc.FullDocument.Px:#,###}**\n" +
                        $"> Last Updated: {doc.FullDocument.Timestamp} (UTC) - {secsAgo:0} secs ago"
                    );
                }
            },
            e => Console.WriteLine($"Error during in Px update watch: {e?.Message}"),
            cancellationToken
        );
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
        var pxAlertChannelId = ConfigHelper.GetDiscordPxAlertChannelId();
        if (await _client.GetChannelAsync(pxAlertChannelId) is not IMessageChannel channel) {
            throw new ArgumentException($"Px alert channel is not a message channel (#{pxAlertChannelId})");
        }

        foreach (var pxCollectionName in await MongoConst.PxTickDatabase.GetCollectionNames()) {
            WatchPxCollection(channel, pxCollectionName, cancellationToken);
        }
    }
}