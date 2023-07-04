using Discord;
using Discord.Interactions;
using MSM.Bot.Utils;
using MSM.Common.Controllers;

namespace MSM.Bot.Handlers.AutoComplete;

public class PxAlertableItemsAutoCompleteHandler : AutocompleteHandler {
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,
        IAutocompleteInteraction interaction,
        IParameterInfo parameter,
        IServiceProvider services
    ) {
        return AutoCompleteHelper.FromStringList(
            interaction,
            (await PxTrackingItemController.GetTrackingItemsAsync()).Select(x => x.Item)
        );
    }
}