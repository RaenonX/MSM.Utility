using Discord.Interactions;
using Eval.net;
using JetBrains.Annotations;
using MSM.Bot.Models;
using MSM.Common.Extensions;

namespace MSM.Bot.Modules.SlashCommands;

[Group("calc", "Commands for calculating things.")]
public class CalcSlashModule : InteractionModuleBase<SocketInteractionContext> {
    [SlashCommand("dpm", "Calculates DPM given boss HP and time left on clear.")]
    [UsedImplicitly]
    public Task DpmCalcAsync(
        [Summary(description: "Boss HP.")] AbbreviatedNumberWrapperModel bossHp,
        [Summary(description: "Minutes left on clear.")] int minsLeft,
        [Summary(description: "Seconds left on clear.")] int secsLeft
    ) =>
        RespondAsync(
            text: $"Overall DPM: {(bossHp.Number / (9 - minsLeft + (60 - secsLeft) / 60m)).ToAbbreviation()}\n" +
                  $"> Boss HP: {bossHp.Number.ToAbbreviation()} - {minsLeft}:{secsLeft:D2} left"
        );

    [SlashCommand("dmg", "Calculates damage based on character stats.")]
    [UsedImplicitly]
    public Task DamageCalcAsync() =>
        RespondAsync(
            text: "https://docs.google.com/spreadsheets/d/1-CrbLIBr_aL7qnpyQWCrOzyBALHnrnyF6FaMY49puEY",
            ephemeral: true
        );

    [SlashCommand("math", "Calculates math expression using Eval.NET.")]
    [UsedImplicitly]
    public Task MathCalcAsync([Summary(description: "Math expression to evaluate.")] string expression) =>
        RespondAsync(
            text: $"Result: **{Evaluator.Execute(expression, EvalConfiguration.DecimalConfiguration)}**\n" +
                  $"> Evaluated expression: {expression}"
        );
}