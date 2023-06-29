using MongoDB.Driver;
using MSM.Common.Models;
using MSM.Common.Utils;

namespace MSM.Common.Controllers;

public static class PxAlertController {
    public static async Task<PxAlertModel?> GetAlert(string item, decimal itemPx) {
        try {
            var utcNow = DateTime.UtcNow;

            return await MongoConst.PxAlertCollection
                .FindOneAndUpdateAsync<PxAlertModel>(
                    // Alert needs to
                    // - Match the item name
                    // - Current time should be >= `NextAlert` meaning it's allowed to send another alert
                    // - Current item price needs to be lower than the alert threshold
                    x => item == x.Item && utcNow >= x.NextAlert && itemPx <= x.MaxPx,
                    // Update `NextAlert` timestamp to the next datetime that allows to send another alert
                    Builders<PxAlertModel>.Update.Set(
                        x => x.NextAlert,
                        utcNow + TimeSpan.FromSeconds(ConfigHelper.GetAlertIntervalSec())
                    ),
                    // Return the alert after update
                    new FindOneAndUpdateOptions<PxAlertModel> {
                        ReturnDocument = ReturnDocument.After
                    }
                );
        } catch (InvalidOperationException e) {
            if (e.Message == "Sequence contains no elements") {
                return null;
            }

            throw;
        }
    }

    public static Task<UpdateResult> SetAlert(string item, decimal maxPx) {
        return MongoConst.PxAlertCollection.UpdateOneAsync(
            x => x.Item == item,
            Builders<PxAlertModel>.Update
                .Set(x => x.NextAlert, DateTime.UtcNow)
                .Set(x => x.MaxPx, maxPx),
            new UpdateOptions { IsUpsert = true }
        );
    }

    public static Task<DeleteResult> DeleteAlert(string item) {
        return MongoConst.PxAlertCollection.DeleteOneAsync(x => x.Item == item);
    }
}