using JetBrains.Annotations;
using MSM.Common.Interfaces;

namespace MSM.TS.Responses;

public record PxBarResponse {
    [UsedImplicitly]
    public required DateTime FetchedAt { get; init; }
    
    [UsedImplicitly]
    public required DateTime? LastUpdated { get; init; }

    [UsedImplicitly]
    public required string Item { get; init; }

    [UsedImplicitly]
    public required IEnumerable<IPxBarModel> Bars { get; init; }
}