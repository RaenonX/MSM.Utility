using MongoDB.Driver;
using MSM.Common.Extensions;
using MSM.Common.Models;

namespace MSM.Common.Controllers;

public static class PxTickController {
    public static Task RecordPx(string item, decimal px) {
        return Task.WhenAll(
            MongoConst.GetPxTickCollection(item).InsertOneAsync(new PxDataModel {
                Timestamp = DateTime.UtcNow,
                Px = px
            }),
            MongoConst.PxMetaCollection.FindOneAndUpdateAsync<PxMetaModel>(
                x => x.Item == item,
                Builders<PxMetaModel>.Update
                    .Set(x => x.LastUpdate, DateTime.UtcNow)
                    .Set(x => x.Px, px),
                new FindOneAndUpdateOptions<PxMetaModel> { IsUpsert = true }
            )
        );
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
            .ToEnumerable();
    }
}