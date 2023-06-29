using MSM.Common.Models;

namespace MSM.Common.Controllers;

public static class PxController {
    public static Task RecordPx(string item, decimal px) {
        return MongoConst.GetPxCollection(item).InsertOneAsync(new PxDataModel {
            Timestamp = DateTime.UtcNow,
            Px = px
        });
    }
}