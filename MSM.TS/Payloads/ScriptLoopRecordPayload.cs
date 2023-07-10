using JetBrains.Annotations;

namespace MSM.TS.Payloads;

public record ScriptLoopRecordPayload(string Token) : IRequireToken {
    [UsedImplicitly]
    public required int Count { get; init; }
    
    [UsedImplicitly]
    public required decimal Elapsed { get; init; }
}