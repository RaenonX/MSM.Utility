using JetBrains.Annotations;

namespace MSM.TS.Responses;

public record ScriptLoopStatsResponse {
    [UsedImplicitly]
    public required decimal AvgItemSec { get; init; }
};