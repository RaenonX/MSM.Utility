using Discord;
using Discord.Interactions;
using JetBrains.Annotations;
using MSM.Bot.Enums;
using MSM.Bot.Handlers.AutoComplete;
using MSM.Common.Controllers;

namespace MSM.Bot.Modules.SlashCommands;

public class PxAlertSlashModule : InteractionModuleBase<SocketInteractionContext> {
    private async Task ShowTradeStationPxCheckAsync() {
        var availableItems = (await PxTickController.GetAvailableItemsAsync())
            .Order()
            .ToList();

        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Pick item(s) for price check")
            .WithMaxValues(2)
            .WithCustomId(SelectMenuId.TradeStationPxCheck.ToString());

        menuBuilder = availableItems
            .Aggregate(
                menuBuilder,
                (current, item) => current.AddOption(label: item, value: item)
            )
            .WithMaxValues(availableItems.Count);

        var builder = new ComponentBuilder()
            .WithSelectMenu(menuBuilder);

        await RespondAsync("Item(s) to price check:", components: builder.Build(), ephemeral: true);
    }

    [SlashCommand("price", "Calls out the UI for Trade Station price check.")]
    [UsedImplicitly]
    public Task CheckTradeStationPriceAsync() => ShowTradeStationPxCheckAsync();

    [SlashCommand("px-chart", "Get the link of the website that shows the pricing chart.")]
    [UsedImplicitly]
    public Task ShowPxChartLinkAsync() => RespondAsync("https://msm.raenonx.cc", ephemeral: true);

    [SlashCommand("px", "Calls out the UI for Trade Station price check.")]
    [UsedImplicitly]
    public Task CheckTradeStationPxAsync() => ShowTradeStationPxCheckAsync();

    [SlashCommand(
        "px-set-alert",
        "Sets a price alert for item on Trade Station. Updates the alert on the item if already exists."
    )]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [UsedImplicitly]
    public async Task SetTradeStationPxAlertAsync(
        [Summary(description: "Target item to trigger the alert.")]
        [Autocomplete(typeof(PxAlertItemAutoCompleteHandler))]
        string item,
        [Summary(description: "Price alert threshold. If the current price falls below this, sends an alert.")]
        decimal maxPx
    ) {
        await PxAlertController.SetAlert(item, maxPx);

        await RespondAsync($"Price alert of **{item}** @ **{maxPx:#,###}** set!");
    }

    [SlashCommand("px-delete-alert", "Deletes a Trade Station price alert.")]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [UsedImplicitly]
    public async Task DeleteTradeStationPxAlertAsync(
        [Summary(description: "Target item to delete the alert.")]
        [Autocomplete(typeof(PxAlertItemAutoCompleteHandler))]
        string item
    ) {
        var result = await PxAlertController.DeleteAlert(item);

        if (result.DeletedCount > 0) {
            await RespondAsync($"Price alert of **{item}** deleted.");
            return;
        }

        await RespondAsync($"Price alert of **{item}** not found.");
    }

    [SlashCommand("px-start-tracking", "Start tracking an item.")]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [UsedImplicitly]
    public async Task StartTrackingItemAsync([Summary(description: "Item name to track.")] string item) {
        var result = await PxTrackingItemController.SetTrackingItemAsync(item);

        if (result.UpsertedId.IsBsonNull) {
            if (result.MatchedCount > 0) {
                await RespondAsync($"Already tracking **{item}**.");
                return;
            }
            
            await RespondAsync($"Failed to start tracking **{item}**.");
            return;
        }

        await RespondAsync($"Started tracking **{item}**!");
    }

    [SlashCommand("px-stop-tracking", "Stop tracking an item.")]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [UsedImplicitly]
    public async Task StopTrackingItemAsync([Summary(description: "Item name to stop tracking.")] string item) {
        var result = await PxTrackingItemController.DeleteTrackingItemAsync(item);

        if (result.DeletedCount > 0) {
            await RespondAsync($"Stopped tracking **{item}**.");
            return;
        }

        await RespondAsync($"Failed to stop tracking **{item}**!");
    }

    [SlashCommand("px-list-tracking", "List currently tracking items.")]
    [DefaultMemberPermissions(GuildPermission.Administrator)]
    [UsedImplicitly]
    public async Task ListTrackingItemsAsync() {
        var result = await PxTrackingItemController.GetTrackingItemsAsync();

        await RespondAsync($"Currently tracking:\n{string.Join('\n', result.Select(x => $"- {x}"))}");
    }
}