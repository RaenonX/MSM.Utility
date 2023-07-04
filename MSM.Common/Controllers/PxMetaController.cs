using System.Linq.Expressions;
using MongoDB.Driver;
using MSM.Common.Models;

namespace MSM.Common.Controllers;

public static class PxMetaController {
    private static async Task<DateTime?> GetLastValidTickUpdate(Expression<Func<PxMetaModel, bool>> condition) {
        var lastTickUpdate = await MongoConst.PxMetaCollection
            .Find(condition)
            .SortByDescending(x => x.LastUpdate)
            .FirstAsync();

        return lastTickUpdate?.LastUpdate;
    }

    public static async Task<PxMetaModel?> GetItemMetaAsync(string item) {
        return await MongoConst.PxMetaCollection
            .Find(x => x.Item == item)
            .SortByDescending(x => x.LastUpdate)
            .FirstAsync();
    }

    public static Task<DateTime?> GetLastValidTickUpdate() {
        return GetLastValidTickUpdate(_ => true);
    }

    public static Task<DateTime?> GetLastValidTickUpdate(string item) {
        return GetLastValidTickUpdate(x => x.Item == item);
    }
}