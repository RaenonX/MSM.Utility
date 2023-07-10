using Discord;
using Discord.Interactions;
using MSM.Bot.Enums;
using MSM.Bot.Extensions;
using MSM.Bot.Workers;
using MSM.Common.Controllers;
using MSM.Common.Extensions;
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

                return $"{x.Key}: {x.Value.Px.ToMesoText()}\n" +
                       $"> Last Updated: {x.Value.Timestamp} (UTC) - {x.Value.Timestamp.ToSecsAgo():0} secs ago";
            })
        );
    }

    public static Embed MakeError(IResult result) {
        return new EmbedBuilder()
            .WithColor(Colors.Danger)
            .WithTitle($"Error - {result.Error}")
            .WithDescription(result.ErrorReason)
            .Build();
    }

    private static EmbedFieldBuilder MakeLastValidTickField(DateTime? lastValidTick) {
        return new EmbedFieldBuilder {
            IsInline = false,
            Name = "Last Valid Tick",
            Value = lastValidTick is null
                ? "(No tick)"
                : $"{lastValidTick.ToSecsAgo():0} secs ago ({lastValidTick} (UTC))"
        };
    }

    public static Embed MakeLastValidTick(DateTime? lastValidTick, Color color) {
        return new EmbedBuilder()
            .WithColor(color)
            .WithFields(MakeLastValidTickField(lastValidTick))
            .Build();
    }

    public static Embed[] MakeSnipingStartWarning(decimal px) {
        return new[] {
            new EmbedBuilder()
                .WithColor(Colors.Danger)
                .WithTitle("Make sure you understand this before start sniping!")
                .WithDescription(string.Join('\n',
                    $"- Price alert will send if price drops below {px.ToMesoText()} i.e. +10% of the given number",
                    "- Sniping agent needs to start too",
                    $"- Sniping session lasts **{PxSnipingItemController.SnipingSessionTimeout.TotalMinutes} minutes**",
                    "- Rerun this command with the current sniping item to extend the sniping session",
                    "- **Turn off** sniping mode **ASAP** after done sniping"
                ))
                .Build(),
            new EmbedBuilder()
                .WithColor(Colors.Warning) // Orange
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

    public static Embed MakePriceAlert(PxMetaModel meta, PxAlertModel alert) {
        return new EmbedBuilder()
            .WithColor(Colors.Info)
            .WithFields(
                new EmbedFieldBuilder {
                    IsInline = true,
                    Name = "Item",
                    Value = meta.Item
                },
                new EmbedFieldBuilder {
                    IsInline = true,
                    Name = "Current Px",
                    Value = meta.Px.ToMesoText()
                },
                new EmbedFieldBuilder {
                    IsInline = false,
                    Name = "Alert Target",
                    Value = MentionUtils.MentionUser(alert.UserId)
                },
                new EmbedFieldBuilder {
                    IsInline = false,
                    Name = "Alert Threshold",
                    Value = alert.MaxPx.ToMesoText()
                },
                new EmbedFieldBuilder {
                    IsInline = false,
                    Name = "Last Updated",
                    Value = $"{meta.LastUpdate.ToSecsAgo():0} secs ago ({meta.LastUpdate} (UTC))"
                }
            )
            .Build();
    }

    private static EmbedFieldBuilder[] MakeSnipingMeta(PxSnipingItemModel sniping) {
        return new[] {
            new EmbedFieldBuilder {
                IsInline = true,
                Name = "Name",
                Value = sniping.Item
            },
            new EmbedFieldBuilder {
                IsInline = true,
                Name = "Snipe Px",
                Value = sniping.Px.ToMesoText()
            }
        };
    }

    public static Embed MakeCurrentSnipingNoUpdate(PxSnipingItemModel sniping, DateTime? lastTickTimestamp) {
        return new EmbedBuilder()
            .WithColor(Colors.Danger)
            .WithFields(MakeSnipingMeta(sniping))
            .WithFields(MakeLastValidTickField(lastTickTimestamp))
            .Build();
    }

    public static async Task<Embed> MakeCurrentSnipingInfo(PxSnipingItemModel sniping) {
        return MakeCurrentSnipingInfo(sniping, await PxMetaController.GetItemMetaAsync(sniping.Item), Colors.Warning);
    }

    public static async Task<Embed> MakeCurrentSnipingInfo(PxSnipingItemModel sniping, Color color) {
        return MakeCurrentSnipingInfo(sniping, await PxMetaController.GetItemMetaAsync(sniping.Item), color);
    }

    public static Embed MakeCurrentSnipingInfo(PxSnipingItemModel sniping, PxMetaModel? meta) {
        return MakeCurrentSnipingInfo(sniping, meta, Colors.Success);
    }

    private static Embed MakeCurrentSnipingInfo(PxSnipingItemModel sniping, PxMetaModel? meta, Color color) {
        return new EmbedBuilder()
            .WithColor(color)
            .WithFields(MakeSnipingMeta(sniping))
            .WithFields(
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