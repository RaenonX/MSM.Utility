using Discord;
using Discord.Interactions;
using JetBrains.Annotations;
using MSM.Bot.Enums;
using MSM.Bot.Handlers.AutoComplete;
using MSM.Common.Controllers;
using MSM.Common.Utils;

namespace MSM.Bot.Modules.SlashCommands;

public class PxAlertSlashModule : InteractionModuleBase<SocketInteractionContext> {
    [SlashCommand("ts", "Checks the current price of items on TS.")]
    [UsedImplicitly]
    public async Task CheckTradeStationPxAsync() {
        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select 1+ item(s)")
            .WithCustomId(SelectMenuId.TradeStationPxCheck.ToString());

        menuBuilder = (await PxTickController.GetAvailableItemsAsync())
            .Aggregate(
                menuBuilder,
                (current, item) => current.AddOption(label: item, value: item)
            );

        var builder = new ComponentBuilder()
            .WithSelectMenu(menuBuilder);

        await ReplyAsync("Pick item(s) to check the current price:", components: builder.Build());
    }

    [SlashCommand(
        "ts-set-alert",
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
        var result = await PxAlertController.SetAlert(item, maxPx);

        if (result.ModifiedCount > 0) {
            await RespondAsync(
                $"Sending price alert every {ConfigHelper.GetAlertIntervalSec()} secs when\n" +
                $"> The price of **{item}** is < **{maxPx:#,###}**"
            );
            return;
        }

        if (result.MatchedCount > 0) {
            await RespondAsync($"Price alert of **{item}** if < **{maxPx:#,###}** exists.");
        }

        await RespondAsync("Price alert not added or modified.");
    }
    
    [SlashCommand("ts-delete-alert", "Deletes a Trade Station price alert.")]
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