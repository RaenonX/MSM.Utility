using JetBrains.Annotations;
using MongoDB.Bson;

namespace MSM.Common.Models; 

public record ScriptLoopTimeModel {
    [UsedImplicitly]
    public ObjectId Id { get; init; }
    
    [UsedImplicitly]
    public required int ItemCount { get; init; }
    
    [UsedImplicitly]
    public required decimal Elapsed { get; init; }
}