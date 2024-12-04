using Discord;
using Discord.Interactions;
using PokerTracker.Service.Interfaces;

namespace PokerTracker.Service.Modules
{
	public class PokerModule(
		ISessionService sessionService) : InteractionModuleBase<SocketInteractionContext>
	{
		[SlashCommand("start", "Start a poker session")]
		[RequireContext(ContextType.Guild)]
		[RequireUserPermission(GuildPermission.ManageChannels)]
		public async Task Start([ChannelTypes(ChannelType.Text)]IChannel channel, [MinValue(0)]int startBalance)
		{
			var result = await sessionService.CreateSession(channel.Id, startBalance);
			if(!result.Success)
			{
				await RespondAsync(result.Message, ephemeral: true);
				return;
			}

			await RespondAsync($"Started a poker session in <#{channel.Id}>, make sure players recieve a starting balance of `{startBalance}`", ephemeral: true);
		}

		[SlashCommand("clear", "Remove all sessions from a channel")]
		[RequireContext(ContextType.Guild)]
		[RequireUserPermission(GuildPermission.ManageChannels)]
		public async Task Start([ChannelTypes(ChannelType.Text)] IChannel channel)
		{
			var result = await sessionService.ClearSessionsInChannel(channel.Id);
			if (!result.Success)
			{
				await RespondAsync(result.Message, ephemeral: true);
				return;
			}

			await RespondAsync($"Removed all sessions in <#{channel.Id}>", ephemeral: true);
		}
	}
}
