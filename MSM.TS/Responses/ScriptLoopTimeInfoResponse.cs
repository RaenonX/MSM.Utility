using JetBrains.Annotations;

namespace MSM.TS.Responses;

public record ScriptLoopTimeInfoResponse {
    [UsedImplicitly]
    public required decimal AvgItemSec { get; init; }
};