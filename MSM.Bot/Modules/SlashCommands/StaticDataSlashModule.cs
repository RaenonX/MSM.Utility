using Discord.Interactions;
using JetBrains.Annotations;
using MSM.Bot.Enums;

namespace MSM.Bot.Modules.SlashCommands;

[Group("static", "Commands for checking static data.")]
public class StaticDataSlashModule : InteractionModuleBase<SocketInteractionContext> {
    [SlashCommand("class", "Returns MSM class data spreadsheet by songj0306.")]
    [UsedImplicitly]
    public Task SendClassDataLinkAsync() =>
        RespondAsync(
            text: "https://docs.google.com/spreadsheets/d/1yUwSaHJmXBOvc-eI0UD7EMVyu4WUc2OwjkXPbMa4AWI",
            ephemeral: true
        );

    [SlashCommand("v3", "Returns MSM vskill 3 of all jobs.")]
    [UsedImplicitly]
    public Task SendV3SkillLinkAsync() =>
        RespondAsync(
            text: "https://docs.google.com/document/d/199FpdTbXf7WI8eOPaOP6qPBCt3-wcOkHaHTDPHIMIos",
            ephemeral: true
        );

    [SlashCommand("ign", "Returns IGN of bijasses.")]
    [UsedImplicitly]
    public Task SendBijassesIgnAsync([Summary(description: "Bijass member name.")] BijassMember bijass) {
        return bijass switch {
            BijassMember.Andrew => RespondAsync(text: "Lungy8"),
            BijassMember.Allen => RespondAsync(text: "Раl"),
            BijassMember.Coolzone => RespondAsync(text: "Rank1WA"),
            BijassMember.Fb => RespondAsync(text: "Мєrcєdєs"),
            BijassMember.Han => RespondAsync(text: "Hаn"),
            BijassMember.Jay => RespondAsync(text: "Вroly"),
            BijassMember.KeviNightLord => RespondAsync(text: "Disavowed"),
            BijassMember.KeviPaladin => RespondAsync(text: "Paliwyn"),
            BijassMember.LilyBishop => RespondAsync(text: "Rosseria"),
            BijassMember.LilyPathfinder => RespondAsync(text: "Roselily"),
            BijassMember.LilyPhantom => RespondAsync(text: "Pixielily"),
            BijassMember.Mikey => RespondAsync(text: "Stimmer"),
            BijassMember.Phil => RespondAsync(text: "DeportedFob"),
            BijassMember.Sheepey => RespondAsync(text: "DeepsNL"),
            BijassMember.TedPhantom => RespondAsync(text: "tєddy"),
            BijassMember.TedShadower => RespondAsync(text: "gаrbage"),
            _ => throw new ArgumentOutOfRangeException(nameof(bijass), bijass, $"Invalid bijass member: {bijass}")
        };
    }
}