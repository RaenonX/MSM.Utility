using Discord;
using MSM.Bot.Enums;

namespace MSM.Bot.Extensions;

public static class InteractionExtensions {
    public static MessageComponent ToPxRefreshButtons(this IEnumerable<string?> items) {
        var builder = new ComponentBuilder();

        builder = items
            .Where(item => item is not null)
            .Aggregate(
                builder,
                (current, item) =>
                    current.WithButton($"Refresh price of {item}", $"{ButtonId.RefreshPx.ToString()}/{item}")
            );

        return builder.Build();
    }
}