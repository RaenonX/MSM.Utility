﻿using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;
using JetBrains.Annotations;
using MUB.Main.Enums;

namespace MUB.Main.Modules;

public partial class MessageModule : InteractionModuleBase<SocketInteractionContext> {
    [GeneratedRegex("<a?:(\\w+):(?:\\w+)>")]
    private static partial Regex SingleEmojiRegex();

    [MessageCommand("Steal Emoji")]
    [UsedImplicitly]
    public async Task StealEmojiAsync(IMessage message) {
        if (message is not IUserMessage userMessage) {
            await RespondAsync(text: "Can't steal emoji from non-user messages!");
            return;
        }

        var content = userMessage.Content;

        if (!content.StartsWith("<") || !content.EndsWith(">")) {
            await RespondAsync(text: "Single emoji message is required!");
            return;
        }

        var contentRegexMatch = SingleEmojiRegex().Match(content);

        if (!contentRegexMatch.Success) {
            await RespondAsync(text: "Emoji regex matching failed!");
            return;
        }

        var emoteName = contentRegexMatch.Groups[1].Value;
        var emote = Emote.Parse(content);

        await RespondWithModalAsync(new ModalBuilder()
            .WithTitle("Emote Stealer")
            .WithCustomId(ModalId.EmoteStealer.ToString())
            .AddTextInput("Emote Name", ModalFieldId.EmoteName.ToString(), value: emoteName)
            .AddTextInput("Emote Link", ModalFieldId.EmoteLink.ToString(), value: emote.Url)
            .Build());
    }
}