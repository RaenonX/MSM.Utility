using JetBrains.Annotations;

namespace MSM.TS.Responses; 

public record AvailableItemsResponse {
    [UsedImplicitly]
    public required IEnumerable<string> Items { get; init; }
}