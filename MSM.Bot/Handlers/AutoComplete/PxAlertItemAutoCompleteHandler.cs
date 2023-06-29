using Discord;
using Discord.Interactions;
using MSM.Common.Controllers;

namespace MSM.Bot.Handlers.AutoComplete;

public class PxAlertItemAutoCompleteHandler : AutocompleteHandler {
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter,
        IServiceProvider services
    ) {
        return AutocompletionResult.FromSuccess(
            (await PxDataController.GetAvailableItemsAsync())
            .Select(x => new AutocompleteResult(x, x))
            // max 25 suggestions at a time (API limit)
            .Take(25)
        );
    }
}