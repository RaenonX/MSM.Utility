using Discord.Interactions;
using JetBrains.Annotations;

namespace MUB.Main.Modules;

public class SlashModule : InteractionModuleBase<SocketInteractionContext> {
    [SlashCommand("dmg-calc", "Calculates overall DPM given boss HP and time left on clear.")]
    [UsedImplicitly]
    public async Task DamageCalcAsync(
        [Summary(description: "Boss HP in B")] double bossHp,
        [Summary(description: "Minutes left on clear")] int minsLeft,
        [Summary(description: "Seconds left on clear")] int secsLeft
    ) =>
        await RespondAsync(
            text: $"Overall DPM: {bossHp / (9 - minsLeft + (60 - secsLeft) / 60f):F3} B\n" +
                  $"> Boss HP: {bossHp:F3} B - {minsLeft}:{secsLeft:D2} left"
        );

    [SlashCommand("ping", "Pings the bot and returns its latency.")]
    [UsedImplicitly]
    public async Task GreetUserAsync() =>
        await RespondAsync(
            text: $"Bot Latency: {Context.Client.Latency} ms",
            ephemeral: true
        );

    [SlashCommand("class", "Returns MSM class data spreadsheet by songj0306.")]
    [UsedImplicitly]
    public async Task SendClassDataLinkAsync() =>
        await RespondAsync(
            text: "https://docs.google.com/spreadsheets/d/1yUwSaHJmXBOvc-eI0UD7EMVyu4WUc2OwjkXPbMa4AWI",
            ephemeral: true
        );

    [SlashCommand("v3", "Returns MSM vskill 3 of all jobs.")]
    [UsedImplicitly]
    public async Task SendV3SkillLinkAsync() =>
        await RespondAsync(
            text: "https://docs.google.com/document/d/199FpdTbXf7WI8eOPaOP6qPBCt3-wcOkHaHTDPHIMIos",
            ephemeral: true
        );
}