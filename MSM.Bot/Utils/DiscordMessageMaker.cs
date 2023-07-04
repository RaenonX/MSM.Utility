﻿using Discord;
using MSM.Bot.Extensions;
using MSM.Bot.Workers;
using MSM.Common.Controllers;
using MSM.Common.Models;

namespace MSM.Bot.Utils;

public static class DiscordMessageMaker {
    public static async Task<string> MakePxReport(IEnumerable<string> items) {
        return string.Join(
            "\n\n",
            (await PxTickController.GetLatestOfItems(items)).Select(x => {
                if (x.Value is null) {
                    return $"{x.Key} does not have price data.";
                }

                var secsAgo = (DateTime.UtcNow - x.Value.Timestamp).TotalSeconds;

                return $"{x.Key}: {x.Value.Px.ToMesoText()}\n" +
                       $"> Last Updated: {x.Value.Timestamp} (UTC) - {secsAgo:0} secs ago";
            })
        );
    }

    public static Embed[] MakeSnipingStartWarning(decimal px) {
        return new[] {
            new EmbedBuilder()
                .WithColor(255, 15, 15) // Red
                .WithTitle("Make sure you understand this before start sniping!")
                .WithDescription(string.Join('\n',
                    $"- Price alert will send if price drops below {px.ToMesoText()} - +10% of the given number",
                    "- Sniping agent needs to start too",
                    $"- Sniping session lasts **{PxSnipingItemController.SnipingSessionTimeout.TotalMinutes} minutes**",
                    "- Rerun this command with the current sniping item to extend the sniping session",
                    "- **Turn off** sniping mode **ASAP** after done sniping"
                ))
                .Build(),
            new EmbedBuilder()
                .WithColor(255, 172, 28) // Orange
                .WithTitle("About Sniping Session")
                .WithDescription(string.Join('\n',
                    "During sniping session, the bot will have these behaviors:",
                    $"- Price check (checking what's the price now) happens every {PxSnipeCheckWorker.CheckInterval.TotalSeconds} secs.",
                    $"- It is considered no price update if the last update happens {PxSnipeCheckWorker.LastValidTickMaxGap.TotalSeconds}+ secs ago.",
                    $"- Price tick check failure alert sends every {PxSnipeCheckWorker.CheckInterval.TotalSeconds} secs."
                ))
                .Build()
        };
    }

    public static async Task<Embed> MakeCurrentSnipingInfo(PxSnipingItemModel sniping) {
        return MakeCurrentSnipingInfo(
            sniping,
            await PxMetaController.GetItemMetaAsync(sniping.Item),
            new Color(255, 234, 0) // Yellow - Pending snipe
        );
    }

    public static async Task<Embed> MakeCurrentSnipingInfo(PxSnipingItemModel sniping, Color color) {
        return MakeCurrentSnipingInfo(sniping, await PxMetaController.GetItemMetaAsync(sniping.Item), color);
    }

    public static Embed MakeCurrentSnipingInfo(PxSnipingItemModel sniping, PxMetaModel? meta) {
        return MakeCurrentSnipingInfo(
            sniping,
            meta,
            new Color(20, 205, 50) // Green - Ready to snipe
        );
    }

    public static Embed MakeCurrentSnipingNoUpdate(PxSnipingItemModel sniping, DateTime? lastTickTimestamp) {
        return new EmbedBuilder()
            .WithColor(new Color(255, 4, 45)) // Red - No update
            .WithFields(
                new EmbedFieldBuilder {
                    IsInline = true,
                    Name = "Name",
                    Value = sniping.Item
                },
                new EmbedFieldBuilder {
                    IsInline = true,
                    Name = "Snipe Px",
                    Value = sniping.Px.ToMesoText()
                },
                new EmbedFieldBuilder {
                    IsInline = false,
                    Name = "Last Valid Tick",
                    Value = lastTickTimestamp is null ? "(No tick)" : $"{lastTickTimestamp} (UTC)"
                }
            )
            .Build();
    }

    private static Embed MakeCurrentSnipingInfo(PxSnipingItemModel sniping, PxMetaModel? meta, Color color) {
        return new EmbedBuilder()
            .WithColor(color)
            .WithFields(
                new EmbedFieldBuilder {
                    IsInline = true,
                    Name = "Name",
                    Value = sniping.Item
                },
                new EmbedFieldBuilder {
                    IsInline = true,
                    Name = "Snipe Px",
                    Value = sniping.Px.ToMesoText()
                },
                new EmbedFieldBuilder {
                    IsInline = false,
                    Name = "Sniping Session Ending Time",
                    Value = $"{sniping.EndingTimestamp} (UTC) - " +
                            $"{(sniping.EndingTimestamp - DateTime.UtcNow).TotalMinutes:0} mins later"
                },
                new EmbedFieldBuilder {
                    IsInline = false,
                    Name = "Current Px",
                    Value = meta?.Px.ToMesoText() ?? "(No data)"
                },
                new EmbedFieldBuilder {
                    IsInline = false,
                    Name = "Last Tick",
                    Value = meta is null ? "(No data)" : $"{meta.LastUpdate} (UTC)"
                }
            )
            .Build();
    }
}