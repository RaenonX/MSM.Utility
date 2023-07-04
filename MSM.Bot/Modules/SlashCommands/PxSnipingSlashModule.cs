using Discord;
using Discord.Interactions;
using JetBrains.Annotations;
using MSM.Bot.Attributes;
using MSM.Bot.Extensions;
using MSM.Bot.Utils;
using MSM.Common.Controllers;

namespace MSM.Bot.Modules.SlashCommands;

[Group("px-snipe", "Commands for sniping items by utilizing the sniping mode.")]
public class PxSnipingSlashModule : InteractionModuleBase<SocketInteractionContext> {
    private const decimal SnipingPriceOffsetPct = 10;
    
    [SlashCommand("start", "Start sniping an item.")]
    [RequiresRoleByConfigKey("PxSnipe")]
    [UsedImplicitly]
    public async Task StartTrackingItemAsync(
        [Summary(description: "Item name to snipe.")] string item,
        [Summary(
            description: "Sniping price threshold. A +15% offset will be applied to mitigate price randomness on TS."
        )]
        decimal px
    ) {
        px *= 1 + SnipingPriceOffsetPct / 100;

        var sniping = await PxSnipingItemController.GetSnipingItemAsync();

        if (sniping is not null) {
            // Already sniping something now
            if (sniping.Item == item) {
                // Sniping the same item
                var updatedSniping = await PxSnipingItemController.SetSnipingItemAsync(item, px);

                await RespondAsync(
                    $"Updated sniping info of **{item}**!",
                    embed: await DiscordMessageMaker.MakeCurrentSnipingInfo(updatedSniping)
                );
                return;
            }

            // Sniping different item
            await RespondAsync(
                $"Already sniping **{sniping.Item}**!",
                embed: await DiscordMessageMaker.MakeCurrentSnipingInfo(sniping)
            );
            return;
        }

        var snipingAlertChannel = await Context.Client.GetSnipingAlertChannelAsync();

        await RespondAsync(
            string.Join('\n',
                $"Are you sure to start sniping **{item}** at {px.ToMesoText()}?",
                $"> All types of alerts (price and system) will send to <#{snipingAlertChannel.Id}>"
            ),
            embeds: DiscordMessageMaker.MakeSnipingStartWarning(px),
            components: item.ToConfirmStartSnipingButton(px)
        );
    }

    [SlashCommand("stop", "Stop sniping an item.")]
    [RequiresRoleByConfigKey("PxSnipe")]
    [UsedImplicitly]
    public async Task StopTrackingItemAsync() {
        var sniping = await PxSnipingItemController.StopSnipingItemAsync();

        if (sniping is null) {
            await RespondAsync("Nothing was not being sniped!");
            return;
        }

        await RespondAsync(
            $"Stopped snipping **{sniping.Item}**!",
            embed: await DiscordMessageMaker.MakeCurrentSnipingInfo(sniping, new Color(169, 169, 169))
        );
    }

    [SlashCommand("show", "Show currently sniping item.")]
    [RequiresRoleByConfigKey("PxSnipe")]
    [UsedImplicitly]
    public async Task ShowSnipingItemAsync() {
        var sniping = await PxSnipingItemController.GetSnipingItemAsync();

        if (sniping is null) {
            await RespondAsync("Not sniping any item now.");
            return;
        }

        await RespondAsync(
            $"Sniping **{sniping.Item}** now!",
            embed: await DiscordMessageMaker.MakeCurrentSnipingInfo(sniping, new Color(0, 150, 255))
        );
    }
}