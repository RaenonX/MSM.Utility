using Discord;
using MSM.Bot.Extensions;
using MSM.Common.Utils;

namespace MSM.Bot.Handlers;

public static class OnLogHandler {
    private static readonly ILogger Logger = LogHelper.CreateLogger(typeof(OnLogHandler));

    public static Task OnLogAsync(LogMessage message) {
        Logger.Log(
            message.Severity.ToLogLevel(),
            message.Exception,
            "{Source}: {Message}",
            message.Source,
            message.Message
        );

        return Task.CompletedTask;
    }
}