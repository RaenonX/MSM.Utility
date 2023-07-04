using Discord;
using Discord.Interactions;
using JetBrains.Annotations;
using MSM.Bot.Enums;
using MSM.Common.Controllers;

namespace MSM.Bot.Modules.SlashCommands; 

[Group("px-check", "Commands for checking various price of items.")]
public class PxCheckingSlashModule : InteractionModuleBase<SocketInteractionContext> {
    [SlashCommand("show", "Calls out the UI for Trade Station price check.")]
    [UsedImplicitly]
    public async Task CheckPxAsync() {
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

    [SlashCommand("chart", "Get the link of the website that shows the pricing chart.")]
    [UsedImplicitly]
    public Task ShowPxChartLinkAsync() => RespondAsync("https://msm.raenonx.cc", ephemeral: true);
}