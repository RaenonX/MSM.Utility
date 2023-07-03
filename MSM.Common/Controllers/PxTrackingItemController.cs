using MongoDB.Driver;
using MSM.Common.Models;

namespace MSM.Common.Controllers;

public static class PxTrackingItemController {
    public static async Task<bool> SetTrackingItemAsync(string item) {
        item = item.Trim();

        return await MongoConst.PxTrackingItemCollection.FindOneAndUpdateAsync(
            Builders<PxTrackingItemModel>.Filter.Where(x => x.Item == item),
            Builders<PxTrackingItemModel>.Update.Set(x => x.Item, item),
            new FindOneAndUpdateOptions<PxTrackingItemModel, PxTrackingItemModel?> {
                IsUpsert = true,
                // Since it's returning `Before`, it is possible that the return of this is null,
                // if the item did not exist
                ReturnDocument = ReturnDocument.Before
            }
        ) is null;
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