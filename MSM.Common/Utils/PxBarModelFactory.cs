using MSM.Common.Interfaces;
using MSM.Common.Models;

namespace MSM.Common.Utils;

public static class PxBarModelFactory {
    public static IPxBarModel FromList(
        IReadOnlyList<PxDataModel> grouping,
        long epochSec,
        decimal? previousClose = null
    ) {
        if (grouping.Count == 0) {
            return new PxBarEmptyModel {
                EpochSecond = epochSec
            };
        }

        var open = grouping.OrderBy(data => data.Timestamp).First().Px;

        return new PxBarModel {
            EpochSecond = epochSec,
            Open = previousClose ?? open,
            High = grouping.Max(data => data.Px),
            Low = grouping.Min(data => data.Px),
            Close = grouping.OrderByDescending(data => data.Timestamp).First().Px,
            // Zip 2 enumerable with offset of 1 (making it comparing "current" and "previous"),
            // and see if there is any difference in price between 2.
            // If so, consider it as a price tick.
            UpTick = grouping.OrderBy(data => data.Timestamp)
                .Zip(grouping.OrderBy(data => data.Timestamp).Skip(1))
                .Select(x => x.Second.Px - x.First.Px > 0 ? 1 : 0)
                .Sum() + (previousClose is not null && open - previousClose > 0 ? 1 : 0),
            DownTick = grouping.OrderBy(data => data.Timestamp)
                .Zip(grouping.OrderBy(data => data.Timestamp).Skip(1))
                .Select(x => x.Second.Px - x.First.Px < 0 ? 1 : 0)
                .Sum() + (previousClose is not null && open - previousClose < 0 ? 1 : 0)
        };
    }
}