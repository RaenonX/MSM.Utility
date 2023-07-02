using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MSM.Common.Utils;

namespace MSM.Bot.Attributes;

public class RequiresRoleByConfigKeyAttribute : PreconditionAttribute {
    private readonly string _key;

    public RequiresRoleByConfigKeyAttribute(string key) => _key = key;

    public override Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        ICommandInfo commandInfo,
        IServiceProvider services
    ) {
        var allowedRoleId = ConfigHelper.GetDiscordRoleIdByKey(_key);

        if (context.User is not SocketGuildUser user) {
            return Task.FromResult(PreconditionResult.FromError("You must be in a guild to run this command."));
        }

        try {
            if (user.Roles.Any(r => r.Id == allowedRoleId)) {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }

            var allowedRoleName = context.Guild.Roles.First(x => x.Id == allowedRoleId).Name;

            return Task.FromResult(
                PreconditionResult.FromError($"You must have the role **{allowedRoleName}** to run this command.")
            );
        } catch (InvalidOperationException) {
            return Task.FromResult(
                PreconditionResult.FromError("The server doesn't have the role allowed to run this command.")
            );
        }
    }
}