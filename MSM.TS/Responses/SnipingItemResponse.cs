using JetBrains.Annotations;
using MSM.Common.Models;

namespace MSM.TS.Responses;

public record SnipingItemResponse {
    [UsedImplicitly]
    public required PxSnipingItemModel? Item { get; init; }
};