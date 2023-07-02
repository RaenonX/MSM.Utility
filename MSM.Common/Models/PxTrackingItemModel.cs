using JetBrains.Annotations;
using MongoDB.Bson.Serialization.Attributes;

namespace MSM.Common.Models;

// To ignore `_id`
[BsonIgnoreExtraElements]
public record PxTrackingItemModel {
    [UsedImplicitly]
    public required string Item { get; init; }
};