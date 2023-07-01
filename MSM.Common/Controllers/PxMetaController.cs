using MongoDB.Driver;

namespace MSM.Common.Controllers; 

public static class PxMetaController {
    public static async Task<DateTime?> GetLastValidTickUpdate() {
        var lastTickUpdate = await MongoConst.PxMetaCollection
            .Find(_ => true)
            .SortByDescending(x => x.LastUpdate)
            .FirstAsync();

        return lastTickUpdate?.LastUpdate;
    }
}