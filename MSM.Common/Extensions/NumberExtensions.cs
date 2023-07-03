namespace MSM.Common.Extensions;

public static class NumberExtensions {
    public static long ToInterval(this long number, int intervalSec) {
        return number / intervalSec * intervalSec;
    }
}