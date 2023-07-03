namespace MSM.Common.Extensions;

public static class NumberExtensions {
    public static long ToInterval(this long number, int intervalSec) {
        return number / intervalSec * intervalSec;
    }

    public static string ToAbbreviation(this decimal number, int decimals = 3) {
        var numberForCheck = Math.Abs(number);

        return numberForCheck switch {
            > 1E9m => $"{(number / 1E9m).ToString($"F{decimals}")} B",
            > 1E6m => $"{(number / 1E6m).ToString($"F{decimals}")} M",
            > 1E3m => $"{(number / 1E3m).ToString($"F{decimals}")} K",
            _ => $"{number.ToString($"F{decimals}")}"
        };
    }
}