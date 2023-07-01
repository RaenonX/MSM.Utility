namespace MSM.Common.Utils;

public class LogHelper {
    internal static ILoggerFactory LoggerFactory { private get; set; } = new LoggerFactory();
    
    internal static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
    
    internal static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);
}