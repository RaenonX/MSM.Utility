using JetBrains.Annotations;
using MongoDB.Bson.Serialization.Attributes;
using MSM.Common.Extensions;

namespace MSM.Common.Models;

// To ignore `_id`
[BsonIgnoreExtraElements]
public record PxBarModel {
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [UsedImplicitly]
    public required DateTime Timestamp { get; init; }
    
    [UsedImplicitly]
    public required decimal Open { get; init; }

    [UsedImplicitly]
    public required decimal High { get; init; }

    [UsedImplicitly]
    public required decimal Low { get; init; }

    [UsedImplicitly]
    public required decimal Close { get; init; }

    [UsedImplicitly]
    public required decimal Tick { get; init; }

    public static PxBarModel FromIGrouping(IGrouping<long, PxDataModel> grouping) {
        return new PxBarModel {
            Timestamp = grouping.Key.ToDateTime(),
            Open = grouping.OrderBy(data => data.Timestamp).First().Px,
            High = grouping.Max(data => data.Px),
            Low = grouping.Min(data => data.Px),
            Close = grouping.OrderByDescending(data => data.Timestamp).First().Px,
            // Zip 2 enumerable with offset of 1 (making it comparing "current" and "previous"),
            // and see if there is any difference in price between 2.
            // If so, consider it as a price tick.
            Tick = grouping.OrderBy(data => data.Timestamp)
                .Zip(grouping.OrderBy(data => data.Timestamp).Skip(1))
                .Select(x => x.Second.Px != x.First.Px ? 1 : 0)
                .Sum()
        };
    }
}