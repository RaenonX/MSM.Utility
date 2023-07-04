using Discord.Interactions;
using JetBrains.Annotations;
using MSM.Bot.Attributes;
using MSM.Bot.Handlers.AutoComplete;
using MSM.Common.Controllers;

namespace MSM.Bot.Modules.SlashCommands;

[Group("px-track", "Commands for tracking prices.")]
public class PxTrackingSlashModule : InteractionModuleBase<SocketInteractionContext> {
    [SlashCommand("start", "Start tracking an item's price.")]
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

    [SlashCommand("stop", "Stop tracking an item's price.")]
    [RequiresRoleByConfigKey("PxAlert")]
    [UsedImplicitly]
    public async Task StopTrackingItemAsync(
        [Summary(description: "Item name to stop tracking its price.")]
        [Autocomplete(typeof(PxTrackingItemsAutoCompleteHandler))]
        string item
    ) {
        var result = await PxTrackingItemController.DeleteTrackingItemAsync(item);

        if (result.DeletedCount > 0) {
            await RespondAsync($"Stopped tracking **{item}**.");
            return;
        }

        await RespondAsync($"**{item}** is not in the tracking list.");
    }
    
    private async Task ListTrackingItemsCommonAsync() {
        var items = (await PxTrackingItemController
                .GetTrackingItemsAsync())
            .Select(x => $"- {x.Item}")
            .ToList();

        if (items.Count == 0) {
            await RespondAsync("Currently not tracking any items.");
        }

        await RespondAsync($"Currently tracking {items.Count} items:\n{string.Join('\n', items)}");
    }

    [SlashCommand("show", "List currently tracking items.")]
    [RequiresRoleByConfigKey("PxAlert")]
    [UsedImplicitly]
    public Task ShowTrackingItemsAsync() => ListTrackingItemsCommonAsync();

    [SlashCommand("list", "List currently tracking items.")]
    [RequiresRoleByConfigKey("PxAlert")]
    [UsedImplicitly]
    public Task ListTrackingItemsAsync() => ListTrackingItemsCommonAsync();
}