using Discord;
using Discord.Interactions;
using JetBrains.Annotations;
using MSM.Bot.Enums;
using MSM.Common.Controllers;

namespace MSM.Bot.Modules.SlashCommands; 

public class PxCheckingSlashModule : InteractionModuleBase<SocketInteractionContext> {
    private async Task ShowTradeStationPxCheckAsync() {
        var availableItems = (await PxTickController.GetAvailableItemsAsync())
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
}