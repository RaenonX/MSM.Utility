using Discord;
using Discord.Interactions;
using MSM.Bot.Models;
using MSM.Common.Utils;

namespace MSM.Bot.Handlers.Types;

public class AbbreviatedNumberTypeConverter : TypeConverter {
    public override bool CanConvertTo(Type type) => typeof(AbbreviatedNumberWrapperModel).IsAssignableFrom(type);

    public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.String;

    public override Task<TypeConverterResult> ReadAsync(
        IInteractionContext context,
        IApplicationCommandInteractionDataOption option,
        IServiceProvider services
    ) {
        if (option.Type != ApplicationCommandOptionType.String) {
            return Task.FromResult(TypeConverterResult.FromError(
                InteractionCommandError.ConvertFailed,
                "Option that accepts abbreviated number can have string value only"
            ));
        }

        try {
            var numString = (string)option.Value;

            return Task.FromResult(TypeConverterResult.FromSuccess(new AbbreviatedNumberWrapperModel {
                Input = numString,
                Number = AbbreviatedNumberParser.Parse(numString)
            }));
        } catch (InvalidAbbreviatedNumberException ex) {
            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ParseFailed, ex.Message));
        }
    }
}