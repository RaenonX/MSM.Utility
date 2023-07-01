using MongoDB.Driver;
using MSM.Common.Models;

namespace MSM.Common.Controllers;

public static class MongoIndexManager {
    public static IEnumerable<Task> Initialize() {
        return new[] {
            PxDataIndex(),
            PxAlertIndex(),
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