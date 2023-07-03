using JetBrains.Annotations;
using MSM.Common.Interfaces;

namespace MSM.Common.Models;

public record PxBarModel : IPxBarModel {
    [UsedImplicitly]
    public required long EpochSecond { get; init; }

    [UsedImplicitly]
    public required decimal Open { get; init; }

    [UsedImplicitly]
    public required decimal High { get; init; }

    [UsedImplicitly]
    public required decimal Low { get; init; }

    [UsedImplicitly]
    public required decimal Close { get; init; }

    [UsedImplicitly]
    public required decimal UpTick { get; init; }

    [UsedImplicitly]
    public required decimal DownTick { get; init; }

    [UsedImplicitly]
    public bool Empty => false;
}