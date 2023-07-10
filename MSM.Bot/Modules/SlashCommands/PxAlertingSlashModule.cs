using Discord;
using Discord.Interactions;
using JetBrains.Annotations;
using MSM.Bot.Attributes;
using MSM.Bot.Extensions;
using MSM.Bot.Handlers.AutoComplete;
using MSM.Bot.Models;
using MSM.Common.Controllers;

namespace MSM.Bot.Modules.SlashCommands;

[Group("px-alert", "Commands for managing price alerts.")]
public class PxAlertingSlashModule : InteractionModuleBase<SocketInteractionContext> {
    [SlashCommand(
        "set",
        "Sets a price alert for item on Trade Station. Updates the alert on the item if already exists."
    )]
    [RequiresRoleByConfigKey("PxAlert")]
    [UsedImplicitly]
    public async Task SetPxAlertAsync(
        [Summary(description: "Target item to trigger the alert.")]
        [Autocomplete(typeof(PxAlertableItemsAutoCompleteHandler))]
        string item,
        [Summary(description: "Price alert threshold. If the current price falls below this, sends an alert.")]
        AbbreviatedNumberWrapperModel maxPx
    ) {
        var startedTracking = await PxTrackingItemController.SetTrackingItemAsync(item);

        await PxAlertController.SetAlert(item, maxPx.Number, Context.User.Id);

        var messageLines = new List<string> {
            $"Price alert of **{item}** @ {maxPx.Number.ToMesoText()} for {Context.User.Mention} set!"
        };
        if (startedTracking) {
            messageLines.Add($"> **{item}** was not being tracked, started tracking now.");
        }

        await RespondAsync(string.Join('\n', messageLines));
    }

    [SlashCommand("delete", "Deletes a Trade Station price alert.")]
    [RequiresRoleByConfigKey("PxAlert")]
    [UsedImplicitly]
    public async Task DeletePxAlertAsync(
        [Summary(description: "Target item to delete the alert.")]
        [Autocomplete(typeof(PxAlertingItemsAutoCompleteHandler))]
        string item
    ) {
        var result = await PxAlertController.DeleteAlert(item, Context.User.Id);

        if (result.DeletedCount > 0) {
            await RespondAsync($"Price alert of **{item}** for {Context.User.Mention} deleted.");
            return;
        }

        await RespondAsync($"Price alert of **{item}** for {Context.User.Mention} not found.");
    }

    private async Task ListPxAlertCommonAsync() {
        var alerts = await PxAlertController.GetAllAlerts();

        if (alerts.Count == 0) {
            await RespondAsync("No active price alerts.");
            return;
        }

        await RespondAsync(
            $"**{alerts.Count}** price alerts in effect:\n" +
            string.Join(
                '\n',
                alerts.Select(x => $"- {x.Item} @ {x.MaxPx.ToMesoText()} by {MentionUtils.MentionUser(x.UserId)}")
            ),
            ephemeral: true,
            allowedMentions: new AllowedMentions { UserIds = new List<ulong>() }
        );
    }

    [SlashCommand("show", "List all price alerts.")]
    [RequiresRoleByConfigKey("PxAlert")]
    [UsedImplicitly]
    public Task ShowPxAlertAsync() => ListPxAlertCommonAsync();

    [SlashCommand("list", "List all price alerts.")]
    [RequiresRoleByConfigKey("PxAlert")]
    [UsedImplicitly]
    public Task ListPxAlertAsync() => ListPxAlertCommonAsync();
}