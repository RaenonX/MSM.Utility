using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MSM.Bot.Enums;
using MSM.Bot.Extensions;
using MSM.Bot.Modules;
using MSM.Bot.Utils;

namespace MSM.Bot.Handlers;

public class InteractionHandler {
    private readonly DiscordSocketClient _client;

    private readonly InteractionService _handler;

    private readonly IServiceProvider _services;

    private readonly IConfiguration _config;

    public InteractionHandler(
        DiscordSocketClient client, InteractionService handler,
        IServiceProvider services, IConfiguration config
    ) {
        _client = client;
        _handler = handler;
        _services = services;
        _config = config;
    }

    public async Task InitializeAsync() {
        _client.Ready += ReadyAsync;
        _handler.Log += OnLogHandler.OnLogAsync;

        await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        _client.InteractionCreated += HandleInteraction;
        _client.ModalSubmitted += HandleModalSubmitted;
    }

    private async Task ReadyAsync() {
        await _handler.RegisterCommandsGloballyAsync();
    }

    private async Task HandleInteraction(SocketInteraction interaction) {
        try {
            var context = new SocketInteractionContext(_client, interaction);

            var result = await _handler.ExecuteCommandAsync(context, _services);

            if (!result.IsSuccess) {
                switch (result.Error) {
                    case InteractionCommandError.UnmetPrecondition:
                        await context.Channel.SendMessageAsync("Command execution meet the precondition.");
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // Ignore unknown command
                        break;
                    case InteractionCommandError.ConvertFailed:
                        await context.Channel.SendMessageAsync("Command argument conversion failed.");
                        break;
                    case InteractionCommandError.BadArgs:
                        await context.Channel.SendMessageAsync("Bad command arguments.");
                        break;
                    case InteractionCommandError.Exception:
                        await context.Channel.SendMessageAsync("App exception occurred during command execution.");
                        break;
                    case InteractionCommandError.Unsuccessful:
                        await context.Channel.SendMessageAsync("Unsuccessful command execution.");
                        break;
                    case InteractionCommandError.ParseFailed:
                        await context.Channel.SendMessageAsync("Command parsing failed.");
                        break;
                    case null:
                        await context.Channel.SendMessageAsync("Unknown command execution error.");
                        break;
                    default:
                        throw new ArgumentException($"Unhandled command execution error: {result.Error}");
                }
            }
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

    private async Task HandleModalSubmitted(SocketModal modal) {
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
}