using JetBrains.Annotations;
using MongoDB.Bson.Serialization.Attributes;

namespace MSM.Common.Models;

// To ignore `_id`
[BsonIgnoreExtraElements]
public record PxSnipingItemModel {
    [UsedImplicitly]
    public required string Item { get; init; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [UsedImplicitly]
    public required DateTime EndingTimestamp { get; init; }

    [UsedImplicitly]
    public required decimal Px { get; init; }
}