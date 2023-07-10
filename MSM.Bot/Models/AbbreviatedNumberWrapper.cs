namespace MSM.Bot.Models;

public record AbbreviatedNumberWrapperModel {
    public required string Input { get; init; }

    public required decimal Number { get; init; }
}