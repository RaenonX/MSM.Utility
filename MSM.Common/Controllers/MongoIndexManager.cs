using MongoDB.Driver;
using MSM.Common.Models;

namespace MSM.Common.Controllers;

public static class MongoIndexManager {
    public static IEnumerable<Task> Initialize() {
        return new[] {
            PxDataIndex(),
            PxAlertIndex(),
            PxTrackingItemIndex(),
            PxSnipingItemIndex(),
            PxMetaIndex()
        };
    }

    private static async Task PxDataIndex() {
        var indexOptions = new CreateIndexOptions {
            Name = "DatetimeSort"
        };
        var indexKeys = Builders<PxDataModel>.IndexKeys
            .Descending(data => data.Timestamp);

        var indexModel = new CreateIndexModel<PxDataModel>(indexKeys, indexOptions);

        await Task.WhenAll(
            (await PxTickController.GetAvailableItemsAsync())
            .Select(symbol => MongoConst.GetPxTickCollection(symbol).Indexes.CreateOneAsync(indexModel))
        );
    }

    private static async Task PxAlertIndex() {
        var indexOptions = new CreateIndexOptions {
            Name = "AlertItem",
            Unique = true
        };
        var indexKeys = Builders<PxAlertModel>.IndexKeys.Ascending(data => data.Item);
        var indexModel = new CreateIndexModel<PxAlertModel>(indexKeys, indexOptions);

        await MongoConst.PxAlertCollection.Indexes.CreateOneAsync(indexModel);
    }

    private static async Task PxTrackingItemIndex() {
        var indexOptions = new CreateIndexOptions {
            Name = "TrackingItem",
            Unique = true
        };
        var indexKeys = Builders<PxTrackingItemModel>.IndexKeys.Ascending(data => data.Item);
        var indexModel = new CreateIndexModel<PxTrackingItemModel>(indexKeys, indexOptions);

        await MongoConst.PxTrackingItemCollection.Indexes.CreateOneAsync(indexModel);
    }

    private static async Task PxSnipingItemIndex() {
        var indexOptions = new CreateIndexOptions {
            Name = "SnipingItem",
            ExpireAfter = TimeSpan.Zero
        };
        var indexKeys = Builders<PxSnipingItemModel>.IndexKeys.Ascending(data => data.EndingTimestamp);
        var indexModel = new CreateIndexModel<PxSnipingItemModel>(indexKeys, indexOptions);

        await MongoConst.PxSnipingItemCollection.Indexes.CreateOneAsync(indexModel);
    }

    private static async Task PxMetaIndex() {
        var indexOptions = new CreateIndexOptions {
            Name = "MetaItem",
            Unique = true
        };
        var indexKeys = Builders<PxMetaModel>.IndexKeys.Ascending(data => data.Item);
        var indexModel = new CreateIndexModel<PxMetaModel>(indexKeys, indexOptions);

        await MongoConst.PxMetaCollection.Indexes.CreateOneAsync(indexModel);
    }
}