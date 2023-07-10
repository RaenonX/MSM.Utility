using JetBrains.Annotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MSM.Common.Models;

public record PxAlertModel {
    [UsedImplicitly]
    public ObjectId Id { get; init; }
    
    [UsedImplicitly]
    public required string Item { get; init; }
    
    [UsedImplicitly]
    public required decimal MaxPx { get; init; }
    
    [UsedImplicitly]
    public required ulong UserId { get; init; }
    
    [UsedImplicitly]
    public required decimal? AlertedAt { get; init; }
    
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    [UsedImplicitly]
    public required DateTime NextAlert { get; init; }
}