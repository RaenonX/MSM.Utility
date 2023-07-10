using MongoDB.Driver;
using MSM.Common.Models;

namespace MSM.Common.Controllers; 

public static class ScriptLoopTimeController {
    public static Task RecordLoopTime(int itemCount, decimal elapsedSec) {
        return MongoConst.ScriptLoopTimeCollection.InsertOneAsync(new ScriptLoopTimeModel {
            ItemCount = itemCount,
            Elapsed = elapsedSec
        });
    }

    public static async Task<decimal> GetAvgPerItem(int count) {
        var loopTimes = await MongoConst.ScriptLoopTimeCollection.Find(_ => true)
            .SortByDescending(x => x.Id)
            .Limit(count)
            .ToListAsync();

        return loopTimes.Sum(x => x.Elapsed) / loopTimes.Sum(x => x.ItemCount);
    }
}