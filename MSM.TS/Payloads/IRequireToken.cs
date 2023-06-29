using JetBrains.Annotations;

namespace MSM.TS.Payloads; 

public interface IRequireToken {
    [UsedImplicitly]
    public string Token { get; init; }
}