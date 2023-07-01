using Discord;
using MSM.Bot.Enums;
using MSM.Common.Utils;

namespace MSM.Bot.Extensions;

public static class DiscordExtensions {
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

    public static async Task<IMessageChannel> GetPxAlertChannel(this IDiscordClient client) {
        var pxAlertChannelId = ConfigHelper.GetDiscordPxAlertChannelId();
        
        if (await client.GetChannelAsync(pxAlertChannelId) is not IMessageChannel channel) {
            throw new ArgumentException($"Px alert channel is not a message channel (#{pxAlertChannelId})");
        }

        return channel;
    }
}