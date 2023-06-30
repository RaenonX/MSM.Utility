namespace MSM.Common.Extensions; 

public static class DateTimeExtensions {
    public static DateTime ToDateTime(this long epochSec) {
        return DateTimeOffset.FromUnixTimeSeconds(epochSec).DateTime;
    }
    
    public static long ToEpochSeconds(this DateTime datetime) {
        return ((DateTimeOffset)datetime).ToUnixTimeSeconds();
    }
}