using Discord;
using MSM.Bot.Enums;
using MSM.Common.Extensions;
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

    public static string ToMesoText(this decimal meso) {
        return meso < 1E6m ? 
            $"**{meso:#,###}**" : 
            $"**{meso.ToAbbreviation(decimals: 2)}** ({meso:#,###})";
    }

    private static async Task<IMessageChannel> GetMessageChannel(this IDiscordClient client, ulong channelId) {
        if (await client.GetChannelAsync(channelId) is not IMessageChannel channel) {
            throw new ArgumentException($"Not a message channel (#{channelId})");
        }

        return channel;
    }

    public static Task<IMessageChannel> GetPxAlertChannelAsync(this IDiscordClient client) {
        return client.GetMessageChannel(ConfigHelper.GetDiscordPxAlertChannelId());
    }

    public static Task<IMessageChannel> GetSystemAlertChannelAsync(this IDiscordClient client) {
        return client.GetMessageChannel(ConfigHelper.GetDiscordSystemAlertChannelId());
    }
}