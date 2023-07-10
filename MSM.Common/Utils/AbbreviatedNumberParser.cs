namespace MSM.Common.Utils;

public class InvalidAbbreviatedNumberException : Exception {
    public InvalidAbbreviatedNumberException(string numString, string reason) : base(
        $"Unable to parse abbreviated number of {numString} into string: {reason}") { }
}

public static class AbbreviatedNumberParser {
    private static readonly Dictionary<char, decimal> Multipliers = new() {
        { 'K', 1E3m },
        { 'M', 1E6m },
        { 'B', 1E9m }
    };

    public static decimal Parse(string numString) {
        numString = numString.Replace(" ", "");

        if (string.IsNullOrEmpty(numString)) {
            throw new InvalidAbbreviatedNumberException(numString, "Empty content");
        }

        numString = numString.ToUpper();

        var suffix = numString[^1];
        var hasSuffix = !char.IsNumber(suffix);
        var gotMultiplier = Multipliers.TryGetValue(suffix, out var multiplier);

        if (hasSuffix && !gotMultiplier) {
            throw new InvalidAbbreviatedNumberException(numString, "Invalid suffix");
        }

        try {
            var number = Convert.ToDecimal(numString[..(hasSuffix ? ^1 : ^0)]);

            return number * (hasSuffix ? multiplier : 1);
        } catch (FormatException) {
            throw new InvalidAbbreviatedNumberException(numString, "Invalid number except suffix");
        }
    }
}