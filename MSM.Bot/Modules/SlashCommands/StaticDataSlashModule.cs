using Discord.Interactions;
using JetBrains.Annotations;
using MSM.Bot.Enums;

namespace MSM.Bot.Modules.SlashCommands; 

public class StaticDataSlashModule : InteractionModuleBase<SocketInteractionContext> {
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