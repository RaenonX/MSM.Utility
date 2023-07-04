namespace MSM.Common.Extensions; 

public static class DateTimeExtensions {
    public static DateTime ToDateTime(this long epochSec) {
        return DateTimeOffset.FromUnixTimeSeconds(epochSec).DateTime;
    }
    
    public static long ToEpochSeconds(this DateTime datetime) {
        return ((DateTimeOffset)datetime).ToUnixTimeSeconds();
    }

    public static double ToSecsAgo(this DateTime datetime) {
        return (DateTime.UtcNow - datetime).TotalSeconds;
    }

    public static double? ToSecsAgo(this DateTime? datetime) {
        if (datetime is null) {
            return null;
        }
        
        return (DateTime.UtcNow - datetime).Value.TotalSeconds;
    }
}