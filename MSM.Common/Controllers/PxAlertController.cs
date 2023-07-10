using MongoDB.Driver;
using MSM.Common.Models;
using MSM.Common.Utils;

namespace MSM.Common.Controllers;

public static class PxAlertController {
    public static async Task<IList<PxAlertModel>> GetAlert(string item, decimal itemPx) {
        try {
            var utcNow = DateTime.UtcNow;

            // Get the triggered alerts
            var alerts = await MongoConst.PxAlertCollection
                // Alert needs to
                // - Match the item name
                // - Current time should be >= `NextAlert` meaning it's allowed to send another alert
                // - Current item price needs to be lower than the alert threshold
                // - Current item price did not trigger alert before
                .Find(x => item == x.Item && utcNow >= x.NextAlert && itemPx <= x.MaxPx && itemPx != x.AlertedAt)
                .ToListAsync();

            // Update alerts that will be triggered later
            await MongoConst.PxAlertCollection
                .UpdateManyAsync(
                    Builders<PxAlertModel>.Filter.In(x => x.Id, alerts.Select(x => x.Id)),
                    // Update `NextAlert` timestamp to the next datetime that allows to send another alert
                    Builders<PxAlertModel>.Update
                        .Set(
                            x => x.NextAlert,
                            utcNow + TimeSpan.FromSeconds(ConfigHelper.GetAlertIntervalSec())
                        )
                        .Set(x => x.AlertedAt, itemPx)
                );

            return alerts;
        } catch (InvalidOperationException e) {
            if (e.Message == "Sequence contains no elements") {
                return Array.Empty<PxAlertModel>();
            }

            throw;
        }
    }

    public static Task<UpdateResult> SetAlert(string item, decimal maxPx, ulong targetUserId) {
        return MongoConst.PxAlertCollection.UpdateOneAsync(
            x => x.Item == item && x.UserId == targetUserId,
            Builders<PxAlertModel>.Update
                .Set(x => x.NextAlert, DateTime.UtcNow)
                .Set(x => x.MaxPx, maxPx)
                .Set(x => x.AlertedAt, null),
            new UpdateOptions { IsUpsert = true }
        );
    }

    public static Task<List<PxAlertModel>> GetAllAlerts() {
        return MongoConst.PxAlertCollection.Find(_ => true).SortBy(x => x.Item).ToListAsync();
    }

    public static Task<DeleteResult> DeleteAlert(string item, ulong targetUserId) {
        return MongoConst.PxAlertCollection.DeleteOneAsync(x => x.Item == item && x.UserId == targetUserId);
    }
}