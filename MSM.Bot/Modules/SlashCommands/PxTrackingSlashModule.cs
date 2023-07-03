using Discord;
using Discord.Interactions;
using JetBrains.Annotations;
using MSM.Bot.Attributes;
using MSM.Bot.Enums;
using MSM.Bot.Handlers.AutoComplete;
using MSM.Common.Controllers;

namespace MSM.Bot.Modules.SlashCommands;

public class PxTrackingSlashModule : InteractionModuleBase<SocketInteractionContext> {
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
    [RequiresRoleByConfigKey("PxAlert")]
    [UsedImplicitly]
    public async Task SetTradeStationPxAlertAsync(
        [Summary(description: "Target item to trigger the alert.")]
        [Autocomplete(typeof(PxAlertItemAutoCompleteHandler))]
        string item,
        [Summary(description: "Price alert threshold. If the current price falls below this, sends an alert.")]
        decimal maxPx
    ) {
        var startedTracking = await PxTrackingItemController.SetTrackingItemAsync(item);

        await PxAlertController.SetAlert(item, maxPx);

        var messageLines = new List<string> { $"Price alert of **{item}** @ **{maxPx:#,###}** set!" };
        if (startedTracking) {
            messageLines.Add($"> **{item}** was not being tracked, started tracking now.");
        }

        await RespondAsync(string.Join('\n', messageLines));
    }

    [SlashCommand("px-delete-alert", "Deletes a Trade Station price alert.")]
    [RequiresRoleByConfigKey("PxAlert")]
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

    [SlashCommand("px-list-alert", "List all price alerts.")]
    [RequiresRoleByConfigKey("PxAlert")]
    [UsedImplicitly]
    public async Task ListTradeStationPxAlertAsync() {
        var alerts = await PxAlertController.GetAllAlerts();

        if (alerts.Count == 0) {
            await RespondAsync("No active price alerts.");
            return;
        }

        await RespondAsync(
            $"**{alerts.Count}** price alerts in effect:\n" +
            $"{string.Join('\n', alerts.Select(x => $"- {x.Item} @ **{x.MaxPx:#,###}**"))}"
        );
    }

    [SlashCommand("px-start-tracking", "Start tracking an item's price.")]
    [RequiresRoleByConfigKey("PxAlert")]
    [UsedImplicitly]
    public async Task StartTrackingItemAsync([Summary(description: "Item name to track the price.")] string item) {
        var startedTracking = await PxTrackingItemController.SetTrackingItemAsync(item);

        if (!startedTracking) {
            await RespondAsync($"Already tracking **{item}**.");
            return;
        }

        await RespondAsync($"Started tracking **{item}**!");
    }

    [SlashCommand("px-stop-tracking", "Stop tracking an item's price.")]
    [RequiresRoleByConfigKey("PxAlert")]
    [UsedImplicitly]
    public async Task StopTrackingItemAsync(
        [Summary(description: "Item name to stop tracking its price.")] string item
    ) {
        var result = await PxTrackingItemController.DeleteTrackingItemAsync(item);

        if (result.DeletedCount > 0) {
            await RespondAsync($"Stopped tracking **{item}**.");
            return;
        }

        await RespondAsync($"Failed to stop tracking **{item}**!");
    }

    [SlashCommand("px-list-tracking", "List currently tracking items.")]
    [RequiresRoleByConfigKey("PxAlert")]
    [UsedImplicitly]
    public async Task ListTrackingItemsAsync() {
        var items = (await PxTrackingItemController
                .GetTrackingItemsAsync())
            .Select(x => $"- {x.Item}")
            .ToList();

        if (items.Count == 0) {
            await RespondAsync("Currently not tracking any items.");
        }

        await RespondAsync($"Currently tracking {items.Count} items:\n{string.Join('\n', items)}");
    }
}