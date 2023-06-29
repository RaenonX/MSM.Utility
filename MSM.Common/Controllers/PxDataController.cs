using MongoDB.Driver;
using MSM.Common.Extensions;
using MSM.Common.Models;

namespace MSM.Common.Controllers;

public static class PxDataController {
    public static Task RecordPx(string item, decimal px) {
        return MongoConst.GetPxCollection(item).InsertOneAsync(new PxDataModel {
            Timestamp = DateTime.UtcNow,
            Px = px
        });
    }

    public static Task<IEnumerable<string>> GetAvailableItemsAsync() {
        return MongoConst.PxDatabase.GetCollectionNames();
    }

    private static async Task<KeyValuePair<string, PxDataModel?>> GetLatestOfItem(string item) {
        var pxData = await MongoConst.GetPxCollection(item)
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
}