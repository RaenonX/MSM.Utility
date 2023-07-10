using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MSM.Bot.Enums;
using MSM.Bot.Extensions;
using MSM.Bot.Handlers.Types;
using MSM.Bot.Models;
using MSM.Bot.Utils;
using MSM.Common.Controllers;

namespace MSM.Bot.Handlers;

public class InteractionHandler {
    private readonly DiscordSocketClient _client;

    private readonly InteractionService _handler;

    private readonly IServiceProvider _services;

    public InteractionHandler(DiscordSocketClient client, InteractionService handler, IServiceProvider services) {
        _client = client;
        _handler = handler;
        _services = services;
    }

    public async Task InitializeAsync() {
        _client.Ready += ReadyAsync;
        _handler.Log += OnLogHandler.OnLogAsync;

        _handler.AddTypeConverter<AbbreviatedNumberWrapperModel>(new AbbreviatedNumberTypeConverter());
        await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        _client.InteractionCreated += OnInteractionCreated;
        _client.ModalSubmitted += OnModalSubmitted;
        _client.SelectMenuExecuted += OnSelectMenuExecuted;
        _client.ButtonExecuted += OnButtonExecuted;

        _handler.SlashCommandExecuted += OnSlashCommandExecuted;
    }

    private async Task ReadyAsync() {
        await _handler.RegisterCommandsGloballyAsync();
    }

    private static Task OnInteractionExecuted(IInteractionContext context, IResult result) {
        if (result.IsSuccess || result.Error == InteractionCommandError.UnknownCommand) {
            // Ignoring unknown command since it's possible to happen for follow up modal interactions
            return Task.CompletedTask;
        }

        return context.Interaction.RespondAsync(
            "Bot error occurred!",
            embed: DiscordMessageMaker.MakeError(result),
            ephemeral: true
        );
    }

    private async Task OnInteractionCreated(SocketInteraction interaction) {
        try {
            var context = new SocketInteractionContext(_client, interaction);

            var result = await _handler.ExecuteCommandAsync(context, _services);

            await OnInteractionExecuted(context, result);
        } catch {
            // If Slash Command execution fails, most likely the original interaction acknowledgement will persist.
            // It is a good idea to delete the original response,
            // or at least let the user know that something went wrong during the command execution.
            if (interaction.Type is InteractionType.ApplicationCommand) {
                await interaction
                    .GetOriginalResponseAsync()
                    .ContinueWith(async msg => await msg.Result.DeleteAsync());
            }
        }
    }

    private static Task OnSlashCommandExecuted(SlashCommandInfo command, IInteractionContext context, IResult result) {
        return OnInteractionExecuted(context, result);
    }

    private async Task OnModalSubmitted(SocketModal modal) {
        var modalId = modal.Data.CustomId.ToModalId();
        var components = modal.Data.Components.ToList();

        switch (modalId) {
            case ModalId.EmoteStealer: {
                var emoteName = components
                    .First(x => x.CustomId.ToModalFieldId() == ModalFieldId.EmoteName).Value;

                if (emoteName is null) {
                    throw new ArgumentException("Emote name should not be empty! (Emote name not found)");
                }

                var emoteLink = components
                    .First(x => x.CustomId.ToModalFieldId() == ModalFieldId.EmoteLink).Value;

                if (emoteLink is null) {
                    throw new ArgumentException("Emote link should not be empty! (Emote link not found)");
                }

                var guildId = modal.GuildId;

                if (guildId is null) {
                    throw new ArgumentException("Guild ID is null from modal!");
                }

                using var client = new HttpClient();

                var emoteHttpResponse = await client.GetAsync(emoteLink);
                var stream = await emoteHttpResponse.Content.ReadAsStreamAsync();

                await _client.GetGuild(guildId.Value).CreateEmoteAsync(
                    emoteName,
                    new Image(stream)
                );
                await modal.RespondAsync($"Emote stolen as **{emoteName}**!");
                break;
            }
            case null:
                return;
            default:
                throw new ArgumentException($"Unhandled modal ID: {modalId}");
        }
    }

    private static async Task OnSelectMenuExecuted(SocketMessageComponent component) {
        var selectMenuId = component.Data.CustomId.ToSelectMenuId();
        var values = component.Data.Values.ToList();

        switch (selectMenuId) {
            case SelectMenuId.TradeStationPxCheck:
                await component.RespondAsync(
                    await DiscordMessageMaker.MakePxReport(values),
                    components: values.ToPxRefreshButtons(),
                    ephemeral: true
                );
                break;
            case null:
                return;
            default:
                throw new ArgumentException($"Unhandled select menu ID: {selectMenuId}");
        }
    }

    private static async Task OnButtonExecuted(SocketMessageComponent component) {
        var idComponents = component.Data.CustomId.Split("/", 2);

        var action = idComponents[0].ToButtonId();
        var actionParameter = idComponents.Length >= 2 ? idComponents[1] : string.Empty;

        switch (action) {
            case ButtonId.RefreshPx:
                await component.RespondAsync(
                    await DiscordMessageMaker.MakePxReport(new[] { actionParameter }),
                    components: new[] { actionParameter }.ToPxRefreshButtons(),
                    ephemeral: true
                );
                break;
            case ButtonId.ConfirmStartSniping:
                var snipingArgs = actionParameter.Split("@", 2);
                var item = snipingArgs[0];
                var px = Convert.ToDecimal(snipingArgs[1]);

                var sniping = await PxSnipingItemController.SetSnipingItemAsync(item, px);

                await component.RespondAsync(
                    $"Started sniping **{item}**!",
                    embed: await DiscordMessageMaker.MakeCurrentSnipingInfo(sniping)
                );
                break;
            case null:
                return;
            default:
                throw new ArgumentException($"Unhandled button ID: {action}");
        }
    }
}