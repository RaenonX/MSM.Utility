using MSM.Common.Controllers;

namespace MSM.Bot.Utils;

public static class DiscordMessageGenerator {
    public static async Task<string> MakePxReport(IEnumerable<string> items) {
        return string.Join(
            "\n\n",
            (await PxTickController.GetLatestOfItems(items)).Select(x => {
                if (x.Value is null) {
                    return $"{x.Key} does not have price data.";
                }

                var secsAgo = (DateTime.UtcNow - x.Value.Timestamp).TotalSeconds;

                return $"{x.Key}: **{x.Value.Px:#,###}**\n" +
                       $"> Last Updated: {x.Value.Timestamp} (UTC) - {secsAgo:0} secs ago";
            })
        );
    }
}