using MSM.Common.Controllers;
using MSM.Common.Extensions;
using MSM.Common.Models;

namespace MSM.Common.Computer;

public static class PxDataAggregator {
    public static async Task<IEnumerable<PxBarModel>> GetBarsAsync(
        string item, DateTime? start, DateTime? end, int intervalMin
    ) {
        var intervalSec = intervalMin * 60;
        
        return (await PxTickController.GetDataBetween(item, start, end))
            .GroupBy(x => x.Timestamp.ToEpochSeconds() / intervalSec * intervalSec)
            .Select(PxBarModel.FromIGrouping);
    }
}