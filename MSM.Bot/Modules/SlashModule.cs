using Discord;
using Discord.Interactions;
using Eval.net;
using JetBrains.Annotations;
using MSM.Bot.Enums;
using MSM.Common.Controllers;

namespace MSM.Bot.Modules;

public class SlashModule : InteractionModuleBase<SocketInteractionContext> {
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

    [SlashCommand("math-calc", "Calculates math expression using Eval.NET.")]
    [UsedImplicitly]
    public async Task MathCalcAsync([Summary(description: "Math expression to evaluate.")] string expression) =>
        await RespondAsync(
            text: $"Result: **{Evaluator.Execute(expression, EvalConfiguration.DecimalConfiguration)}**\n" +
                  $"> Evaluated expression: {expression}"
        );

    [SlashCommand("dmg-calc", "Calculates damage based on character stats.")]
    [UsedImplicitly]
    public async Task DamageCalcAsync() =>
        await RespondAsync(
            text: "https://docs.google.com/spreadsheets/d/1-CrbLIBr_aL7qnpyQWCrOzyBALHnrnyF6FaMY49puEY",
            ephemeral: true
        );

    [SlashCommand("ping", "Pings the bot and returns its latency.")]
    [UsedImplicitly]
    public async Task PingAsync() =>
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

    [SlashCommand("ts", "Checks the current price of items on TS.")]
    [UsedImplicitly]
    public async Task CheckTradeStationPxAsync() {
        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select 1+ item(s)")
            .WithCustomId(SelectMenuId.TradeStationPxCheck.ToString());

        menuBuilder = (await PxController.GetAvailableItemsAsync())
            .Aggregate(
                menuBuilder,
                (current, item) => current.AddOption(label: item, value: item)
            );

        var builder = new ComponentBuilder()
            .WithSelectMenu(menuBuilder);

        await ReplyAsync("Pick item(s) to check the current price:", components: builder.Build());
    }

    [SlashCommand("ign", "Returns IGN of bijasses.")]
    [UsedImplicitly]
    public async Task SendBijassesIgnAsync([Summary(description: "Bijass member name.")] BijassMember bijass) {
        switch (bijass) {
            case BijassMember.Andrew:
                await RespondAsync(text: "Lungy8");
                break;
            case BijassMember.Allen:
                await RespondAsync(text: "Раl");
                break;
            case BijassMember.Coolzone:
                await RespondAsync(text: "Rank1WA");
                break;
            case BijassMember.Fb:
                await RespondAsync(text: "Мєrcєdєs");
                break;
            case BijassMember.Han:
                await RespondAsync(text: "Hаn");
                break;
            case BijassMember.Jay:
                await RespondAsync(text: "Вroly");
                break;
            case BijassMember.KeviNightLord:
                await RespondAsync(text: "Disavowed");
                break;
            case BijassMember.KeviPaladin:
                await RespondAsync(text: "Paliwyn");
                break;
            case BijassMember.LilyBishop:
                await RespondAsync(text: "Rosseria");
                break;
            case BijassMember.LilyPathfinder:
                await RespondAsync(text: "Roselily");
                break;
            case BijassMember.LilyPhantom:
                await RespondAsync(text: "Pixielily");
                break;
            case BijassMember.Mikey:
                await RespondAsync(text: "Stimmer");
                break;
            case BijassMember.Phil:
                await RespondAsync(text: "DeportedFob");
                break;
            case BijassMember.Sheepey:
                await RespondAsync(text: "DeepsNL");
                break;
            case BijassMember.TedPhantom:
                await RespondAsync(text: "tєddy");
                break;
            case BijassMember.TedShadower:
                await RespondAsync(text: "gаrbage");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(bijass), bijass, $"Invalid bijass member: {bijass}");
        }
    }
}