using Discord;
using Discord.Interactions;

namespace MSM.Bot.Utils;

public static class AutoCompleteHelper {
    public static AutocompletionResult FromStringList(
        IAutocompleteInteraction interaction,
        IEnumerable<string> options
    ) {
        var value = interaction.Data.Current.Value;

        if (value is not string enteredValue) {
            return AutocompletionResult.FromError(new InvalidCastException(
                $"Unable to cast autocomplete option value to string (Value: {value})"
            ));
        }

        return AutocompletionResult.FromSuccess(
            options
                .Where(x => x.Contains(enteredValue))
                .Select(x => new AutocompleteResult(x, x))
                // max 25 suggestions at a time (API limit)
                .Take(25)
        );
    }
}