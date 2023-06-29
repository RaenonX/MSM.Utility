using JetBrains.Annotations;

namespace MSM.TS.Payloads;

public record PxRecordPayload(string Token) : IRequireToken {
    [UsedImplicitly]
    public required string Item { get; init; }
    
    [UsedImplicitly]
    public required decimal Px { get; init; }
}