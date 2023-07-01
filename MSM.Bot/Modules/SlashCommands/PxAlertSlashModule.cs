using Discord;
using Discord.Interactions;
using JetBrains.Annotations;
using MSM.Bot.Enums;
using MSM.Bot.Handlers.AutoComplete;
using MSM.Common.Controllers;
using MSM.Common.Utils;

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

        await RespondAsync(
            $"Sending price alert every {ConfigHelper.GetAlertIntervalSec()} secs when\n" +
            $"> The price of **{item}** is < **{maxPx:#,###}**"
        );
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
}