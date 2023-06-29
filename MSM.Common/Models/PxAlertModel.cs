using JetBrains.Annotations;
using MongoDB.Bson.Serialization.Attributes;

namespace MSM.Common.Models;

// To ignore `_id`
[BsonIgnoreExtraElements]
public record PxAlertModel {
    [UsedImplicitly]
    public required string Item { get; init; }
    
    [UsedImplicitly]
    public required decimal MaxPx { get; init; }
    
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [UsedImplicitly]
    public required DateTime NextAlert { get; init; }
}