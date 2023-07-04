using MongoDB.Driver;
using MSM.Common.Extensions;
using MSM.Common.Models;

namespace MSM.Common.Controllers;

public static class PxSnipingItemController {
    // Usually an item will be posted within 80 mins on TS
    public static readonly TimeSpan SnipingSessionTimeout = TimeSpan.FromMinutes(80);

    public static Task<PxSnipingItemModel?> GetSnipingItemAsync() {
        return MongoConst.PxSnipingItemCollection.Find(_ => true).SingleOrNullAsync();
    }

    public static Task<PxSnipingItemModel> SetSnipingItemAsync(string item, decimal px) {
        return MongoConst.PxSnipingItemCollection.FindOneAndUpdateAsync<PxSnipingItemModel>(
            _ => true,
            Builders<PxSnipingItemModel>.Update
                .Set(x => x.Item, item)
                .Set(x => x.Px, px)
                .Set(x => x.EndingTimestamp, DateTime.UtcNow + SnipingSessionTimeout),
            new FindOneAndUpdateOptions<PxSnipingItemModel> {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            }
        );
    }

    public static async Task<PxSnipingItemModel?> StopSnipingItemAsync() {
        var result = await MongoConst.PxSnipingItemCollection.FindOneAndDeleteAsync(_ => true);

        return result;
    }
}