using Microsoft.Extensions.Logging.Console;

namespace MSM.Common.Utils;

public static class LogHelper {
    public static readonly Action<SimpleConsoleFormatterOptions> LoggingConfigureAction = options => {
        options.SingleLine = true;
        options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
    };

    internal static ILoggerFactory Factory { private get; set; } = LoggerFactory.Create(
        builder => builder.AddSimpleConsole(LoggingConfigureAction)
    );

    public static ILogger CreateLogger<T>() => Factory.CreateLogger<T>();

    public static ILogger CreateLogger(Type @class) =>
        Factory.CreateLogger(@class.FullName ?? @class.Assembly.Location);

    public static ILogger CreateLogger(string categoryName) => Factory.CreateLogger(categoryName);
}