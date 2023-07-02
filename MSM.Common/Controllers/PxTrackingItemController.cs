using MongoDB.Driver;
using MSM.Common.Models;

namespace MSM.Common.Controllers;

public static class PxTrackingItemController {
    public static Task<UpdateResult> SetTrackingItemAsync(string item) {
        return MongoConst.PxTrackingItemCollection.UpdateOneAsync(
            Builders<PxTrackingItemModel>.Filter.Where(x => x.Item == item),
            Builders<PxTrackingItemModel>.Update.Set(x => x.Item, item),
            new UpdateOptions { IsUpsert = true }
        );
    }

    public static Task<List<PxTrackingItemModel>> GetTrackingItemsAsync() {
        return MongoConst.PxTrackingItemCollection.Find(_ => true)
                .SortBy(x => x.Item)
                .ToListAsync();
    }

    public static Task<DeleteResult> DeleteTrackingItemAsync(string item) {
        return MongoConst.PxTrackingItemCollection.DeleteOneAsync(
            Builders<PxTrackingItemModel>.Filter.Where(x => x.Item == item)
        );
    }
}