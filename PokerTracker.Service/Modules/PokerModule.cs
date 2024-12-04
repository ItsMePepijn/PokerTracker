using Discord;
using Discord.Interactions;

namespace PokerTracker.Service.Modules
{
	public class PokerModule : InteractionModuleBase<SocketInteractionContext>
	{
		[SlashCommand("start", "Start a poker session")]
		[RequireContext(ContextType.Guild)]
		[RequireUserPermission(GuildPermission.ManageChannels)]
		public async Task Start()
		{
			await RespondAsync($"{Context.Interaction.User.Mention} I will start a poker session!");
		}
	}
}
