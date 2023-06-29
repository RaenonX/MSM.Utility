using MongoDB.Driver;
using MSM.Common.Extensions;
using MSM.Common.Models;

namespace MSM.Common.Controllers;

public static class MongoIndexManager {
    public static IEnumerable<Task> Initialize() {
        return new[] { PxLookup() };
    }

    private static async Task PxLookup() {
        var indexOptions = new CreateIndexOptions {
            Name = "DatetimeSort"
        };
        var indexKeys = Builders<PxDataModel>.IndexKeys
            .Descending(data => data.Timestamp);
        
        var indexModel = new CreateIndexModel<PxDataModel>(indexKeys, indexOptions);

        await Task.WhenAll(
            (await MongoConst.PxDatabase.GetCollectionNames())
            .Select(symbol => MongoConst.GetPxCollection(symbol).Indexes.CreateOneAsync(indexModel))
        );
    }
}