using JetBrains.Annotations;
using MSM.Common.Models;

namespace MSM.TS.Responses;

public record PxBarResponse {
    [UsedImplicitly]
    public required string Item { get; init; }

    [UsedImplicitly]
    public required IEnumerable<PxBarModel> Bars { get; init; }
}