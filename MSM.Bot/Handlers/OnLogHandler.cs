using Discord;

namespace MSM.Bot.Handlers;

public static class OnLogHandler {
    public static Task OnLogAsync(LogMessage message) {
        Console.WriteLine(message.ToString());
        return Task.CompletedTask;
    }
}