using MSM.Common.Controllers;
using MSM.Common.Extensions;
using MSM.Common.Interfaces;
using MSM.Common.Models;
using MSM.Common.Utils;

namespace MSM.Common.Computer;

public static class PxDataAggregator {
    public static IEnumerable<IPxBarModel> GetBars(
        string item, DateTime? start, DateTime? end, int intervalMin
    ) {
        var intervalSec = intervalMin * 60;

        using var enumerator = PxTickController.GetDataBetween(item, start, end).GetEnumerator();

        // Move to get the first element - return empty enumerable if failed to advance to the first element
        if (!enumerator.MoveNext()) {
            yield break;
        }

        // Track currently calculating tick(s)
        var tick = enumerator.Current;
        var tickEpochSec = tick.Timestamp.ToEpochSeconds().ToInterval(intervalSec);
        var ticks = new List<PxDataModel> { tick };
        // Define bar range - start from initial tick available
        var epochSecOfCurrent = tick.Timestamp.ToEpochSeconds().ToInterval(intervalSec);
        var epochSecOfNext = epochSecOfCurrent + intervalSec;
        PxBarModel? previousNonEmptyBar = null;
        var cursorMoved = true;

        while (cursorMoved && (end is null || epochSecOfCurrent < end.Value.ToEpochSeconds())) {
            while (tickEpochSec >= epochSecOfCurrent && tickEpochSec < epochSecOfNext) {
                cursorMoved = enumerator.MoveNext();
                if (!cursorMoved) {
                    break;
                }
                
                tick = enumerator.Current;
                tickEpochSec = tick.Timestamp.ToEpochSeconds().ToInterval(intervalSec);

                ticks.Add(tick);
            }

            var yieldedBar = PxBarModelFactory.FromList(
                ticks,
                epochSecOfCurrent, 
                previousClose: previousNonEmptyBar?.Close
            );
            yield return yieldedBar;

            ticks.Clear();
            epochSecOfCurrent = epochSecOfNext;
            epochSecOfNext = epochSecOfCurrent + intervalSec;
            if (yieldedBar is PxBarModel currentNonEmptyBar) {
                previousNonEmptyBar = currentNonEmptyBar;
            }
        }
    }
}