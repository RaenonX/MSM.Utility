using Discord;
using Discord.Interactions;
using JetBrains.Annotations;
using MSM.Bot.Enums;
using MSM.Common.Controllers;

namespace MSM.Bot.Modules.SlashCommands; 

public class PxAlertSlashModule : InteractionModuleBase<SocketInteractionContext> {
    [SlashCommand("ts", "Checks the current price of items on TS.")]
    [UsedImplicitly]
    public async Task CheckTradeStationPxAsync() {
        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select 1+ item(s)")
            .WithCustomId(SelectMenuId.TradeStationPxCheck.ToString());

        menuBuilder = (await PxDataController.GetAvailableItemsAsync())
            .Aggregate(
                menuBuilder,
                (current, item) => current.AddOption(label: item, value: item)
            );

        var builder = new ComponentBuilder()
            .WithSelectMenu(menuBuilder);

        await ReplyAsync("Pick item(s) to check the current price:", components: builder.Build());
    }
}