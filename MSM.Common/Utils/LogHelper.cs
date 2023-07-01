using Microsoft.Extensions.Logging.Console;

namespace MSM.Common.Utils;

public static class LogHelper {
    public static readonly Action<SimpleConsoleFormatterOptions> LoggingConfigureAction = options => {
        // It's better NOT to enable single line because error logs are single line, making it harder to read
        options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
        options.UseUtcTimestamp = true;
    };

    internal static ILoggerFactory Factory { private get; set; } = LoggerFactory.Create(
        builder => builder.AddSimpleConsole(LoggingConfigureAction)
    );

    public static ILogger CreateLogger<T>() => Factory.CreateLogger<T>();

    public static ILogger CreateLogger(Type @class) =>
        Factory.CreateLogger(@class.FullName ?? @class.Assembly.Location);

    public static ILogger CreateLogger(string categoryName) => Factory.CreateLogger(categoryName);
}