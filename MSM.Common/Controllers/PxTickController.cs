using System.Collections.Concurrent;
using MongoDB.Driver;
using MSM.Common.Enums;
using MSM.Common.Extensions;
using MSM.Common.Models;
using MSM.Common.Utils;

namespace MSM.Common.Controllers;

public static class PxTickController {
    private static readonly ConcurrentDictionary<string, ConcurrentQueue<PxDataModel>> VerificationPendingQueue = new();

    private static readonly ILogger Logger = LogHelper.CreateLogger(typeof(PxTickController));

    private const decimal PxVerificationThresholdPct = 5;

    private const int PxVerificationRequiredCount = 3;

    private const int PxVerificationTimeoutMin = 5;

    private static Task RecordPx(
        IMongoCollection<PxDataModel> collection,
        ConcurrentQueue<PxDataModel> queue,
        string item,
        PxDataModel incoming,
        bool abortQueue
    ) {
        var recordPxTask = queue.IsEmpty || abortQueue
            ? collection.InsertOneAsync(incoming)
            : collection.InsertManyAsync(queue.Concat(new[] { incoming }));
        var metaUpdateTask = MongoConst.PxMetaCollection.FindOneAndUpdateAsync<PxMetaModel>(
            x => x.Item == item,
            Builders<PxMetaModel>.Update
                .Set(x => x.LastUpdate, DateTime.UtcNow)
                .Set(x => x.Px, incoming.Px),
            new FindOneAndUpdateOptions<PxMetaModel> { IsUpsert = true }
        );

        Logger.LogInformation(
            "Received {Item} at {Px} ({VerificationResult})",
            item,
            incoming.Px,
            abortQueue ? "Abort" : queue.IsEmpty ? "Clean" : "Verified"
        );

        queue.Clear();

        return Task.WhenAll(recordPxTask, metaUpdateTask);
    }

    public static async Task<PxRecordResult> RecordPx(string item, decimal px) {
        var collection = MongoConst.GetPxTickCollection(item);
        var verificationQueue = VerificationPendingQueue.GetOrAdd(item, new ConcurrentQueue<PxDataModel>());

        var incoming = new PxDataModel {
            Timestamp = DateTime.UtcNow,
            Px = px
        };

        verificationQueue.TryPeek(out var latestInQueue);

        var latestInDb = await collection
            .Find(_ => true)
            .SortByDescending(x => x.Timestamp)
            .Limit(1)
            .SingleOrDefaultAsync();
        var latest = latestInQueue ?? latestInDb ?? null;

        if (
            // Empty collection / new data, could be a glitch
            (verificationQueue.IsEmpty && await collection.EstimatedDocumentCountAsync() == 0) ||
            // Price diff > threshold / large price fluctuation could indicate it checked wrong item
            (latest is not null && MathHelper.DifferencePct(latest.Px, px) > PxVerificationThresholdPct) ||
            // Verification queue is not empty and the data count is not enough / verification in progress
            verificationQueue is { IsEmpty: false, Count: < PxVerificationRequiredCount - 1 }
        ) {
            // Price diff < threshold comparing latest in DB / current data in queue is incorrect
            if (latestInDb is not null && MathHelper.DifferencePct(latestInDb.Px, px) < PxVerificationThresholdPct) {
                await RecordPx(collection, verificationQueue, item, incoming, abortQueue: true);

                return PxRecordResult.RecordedWithQueueAborted;
            }

            // Remove timed-out entries in the verification queue
            verificationQueue.TryPeek(out var queueTop);
            while (
                queueTop is not null &&
                DateTime.UtcNow - queueTop.Timestamp > TimeSpan.FromMinutes(PxVerificationTimeoutMin)
            ) {
                verificationQueue.TryDequeue(out var dequeued);
                
                Logger.LogInformation(
                    "Removed timed out entry of {Item} from queue ({PoppedTimestamp} @ {Px})",
                    item,
                    dequeued?.Timestamp,
                    dequeued?.Px
                );
                
                verificationQueue.TryPeek(out queueTop);
            }

            // Insert incoming data into verification queue
            verificationQueue.Enqueue(incoming);

            Logger.LogInformation(
                "Received {Item} at {Px} ({CountToVerify} in verification queue)",
                item,
                px,
                verificationQueue.Count
            );

            return PxRecordResult.VerificationPending;
        }

        await RecordPx(collection, verificationQueue, item, incoming, abortQueue: false);

        return PxRecordResult.Recorded;
    }

    public static Task<IEnumerable<string>> GetAvailableItemsAsync() {
        return MongoConst.PxTickDatabase.GetCollectionNames();
    }

    private static async Task<KeyValuePair<string, PxDataModel?>> GetLatestOfItem(string item) {
        var pxData = await MongoConst.GetPxTickCollection(item)
            .Find(_ => true)
            .SortByDescending(x => x.Timestamp)
            .Limit(1)
            .SingleAsync();

        return new KeyValuePair<string, PxDataModel?>(item, pxData);
    }

    public static async Task<Dictionary<string, PxDataModel?>> GetLatestOfItems(IEnumerable<string> items) {
        return (await Task.WhenAll(items.Select(GetLatestOfItem)))
            .ToDictionary(
                x => x.Key,
                x => x.Value
            );
    }

    public static async Task<IEnumerable<PxDataModel>> GetDataBetween(string item, DateTime start, DateTime end) {
        return (await MongoConst.GetPxTickCollection(item).FindAsync(x => x.Timestamp >= start && x.Timestamp <= end))
            .ToEnumerable()
            .OrderBy(x => x.Timestamp);
    }
}