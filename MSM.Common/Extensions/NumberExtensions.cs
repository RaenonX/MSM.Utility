namespace MSM.Common.Extensions;

public static class NumberExtensions {
    public static long ToInterval(this long number, int intervalSec) {
        return number / intervalSec * intervalSec;
    }

    public static string ToAbbreviation(this decimal number, int decimals = 3) {
        if (number > 1E9m) {
            return $"{(number / 1E9m).ToString($"F{decimals}")} B";
        }

        if (number > 1E6m) {
            return $"{(number / 1E6m).ToString($"F{decimals}")} M";
        }
        
        if (number > 1E3m) {
            return $"{(number / 1E3m).ToString($"F{decimals}")} K";
        }

        return $"{number.ToString($"F{decimals}")}";
    }
}