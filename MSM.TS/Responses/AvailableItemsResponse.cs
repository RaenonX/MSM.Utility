namespace MSM.TS.Responses; 

public record AvailableItemsResponse {
    public required IEnumerable<string> Items { get; init; }
}