using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MSM.Bot.Enums;
using MSM.Bot.Extensions;
using MSM.Bot.Utils;

namespace MSM.Bot.Handlers;

public class InteractionHandler {
    private readonly DiscordSocketClient _client;

    private readonly InteractionService _handler;

    private readonly IServiceProvider _services;

    private readonly IHostEnvironment _environment;

    public InteractionHandler(
        DiscordSocketClient client, InteractionService handler,
        IServiceProvider services, IHostEnvironment environment
    ) {
        _client = client;
        _handler = handler;
        _services = services;
        _environment = environment;
    }

    public async Task InitializeAsync() {
        _client.Ready += ReadyAsync;
        _handler.Log += OnLogHandler.OnLogAsync;

        await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        _client.InteractionCreated += OnInteractionCreated;
        _client.ModalSubmitted += OnModalSubmitted;
        _client.SelectMenuExecuted += OnSelectMenuExecuted;
        _client.ButtonExecuted += OnButtonExecuted;
        
        _handler.SlashCommandExecuted += OnSlashCommandExecuted;
    }

    private async Task ReadyAsync() {
        if (_environment.IsDevelopment())
            await _handler.RegisterCommandsToGuildAsync(1100607979734188043);
        else {
            await _handler.RegisterCommandsGloballyAsync();
        }
    }

    private static Task OnInteractionExecuted(IInteractionContext context, IResult result) {
        return result.IsSuccess
            ? Task.CompletedTask
            : context.Interaction.RespondAsync($"{result.Error}: {result.ErrorReason}", ephemeral: true);
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
                    await DiscordMessageGenerator.MakePxReport(values),
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
                    await DiscordMessageGenerator.MakePxReport(new[] { actionParameter }),
                    components: new[] { actionParameter }.ToPxRefreshButtons(),
                    ephemeral: true
                );
                break;
            case null:
                return;
            default:
                throw new ArgumentException($"Unhandled button ID: {action}");
        }
    }
}