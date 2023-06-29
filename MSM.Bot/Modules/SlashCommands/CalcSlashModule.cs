using Discord.Interactions;
using Eval.net;
using JetBrains.Annotations;

namespace MSM.Bot.Modules.SlashCommands;

public class CalcSlashModule : InteractionModuleBase<SocketInteractionContext> {
    [SlashCommand("dpm-calc", "Calculates DPM given boss HP and time left on clear.")]
    [UsedImplicitly]
    public async Task DpmCalcAsync(
        [Summary(description: "Boss HP in B.")] double bossHp,
        [Summary(description: "Minutes left on clear.")] int minsLeft,
        [Summary(description: "Seconds left on clear.")] int secsLeft
    ) =>
        await RespondAsync(
            text: $"Overall DPM: {bossHp / (9 - minsLeft + (60 - secsLeft) / 60f):F3} B\n" +
                  $"> Boss HP: {bossHp:F3} B - {minsLeft}:{secsLeft:D2} left"
        );

    [SlashCommand("dmg-calc", "Calculates damage based on character stats.")]
    [UsedImplicitly]
    public async Task DamageCalcAsync() =>
        await RespondAsync(
            text: "https://docs.google.com/spreadsheets/d/1-CrbLIBr_aL7qnpyQWCrOzyBALHnrnyF6FaMY49puEY",
            ephemeral: true
        );

    [SlashCommand("math-calc", "Calculates math expression using Eval.NET.")]
    [UsedImplicitly]
    public async Task MathCalcAsync([Summary(description: "Math expression to evaluate.")] string expression) =>
        await RespondAsync(
            text: $"Result: **{Evaluator.Execute(expression, EvalConfiguration.DecimalConfiguration)}**\n" +
                  $"> Evaluated expression: {expression}"
        );
}