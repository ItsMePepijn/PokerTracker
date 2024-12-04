using Discord;
using Discord.WebSocket;
using PokerTracker.Common.Extensions;
using PokerTracker.Common.Models;
using PokerTracker.Service.Interfaces;
using Results = PokerTracker.Common.Results;

namespace PokerTracker.Service.Services
{
	public class MessageService(
		DiscordSocketClient discord,
		ILogger<MessageService> logger) : IMessageService
	{
		public async Task<Result<IUserMessage?>> SendMessageToChannelAsync(
			ulong channelId,
			string? text = null,
			bool isTTS = false,
			Embed? embed = null,
			RequestOptions? options = null,
			AllowedMentions? allowedMentions = null,
			MessageReference? messageReference = null,
			MessageComponent? components = null,
			ISticker[]? stickers = null,
			Embed[]? embeds = null,
			MessageFlags flags = MessageFlags.None,
			PollProperties? poll = null)
		{
			try
			{
				var channel = discord.GetChannel(channelId);

				if (channel is null)
				{
					return Results.ChannelNotFound.Null<IUserMessage?>();
				}

				if (channel is not IMessageChannel messageChannel)
				{
					return Results.ChannelNotText.Null<IUserMessage?>();
				}

				var message = await messageChannel.SendMessageAsync(
					text,
					isTTS,
					embed,
					options,
					allowedMentions,
					messageReference,
					components,
					stickers,
					embeds,
					flags,
					poll);

				return Results.Success.Of<IUserMessage?>(message);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "{method}: Failed to send message to channel: {ex}", nameof(SendMessageToChannelAsync), ex);
				return Results.Failure.Null<IUserMessage?>();
			}
		}

		public async Task<Result> DeleteMessageFromChannel(ulong channelId, ulong messageId)
		{
			try
			{
				var channel = discord.GetChannel(channelId);
				if (channel is null)
				{
					return Results.ChannelNotFound;
				}

				if (channel is not IMessageChannel messageChannel)
				{
					return Results.ChannelNotText;
				}

				await messageChannel.DeleteMessageAsync(messageId);

				return Results.Success;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "{method}: Failed to delete message from channel: {ex}", nameof(DeleteMessageFromChannel), ex);
				return Results.Failure;
			}
		}

		public async Task<Result> ModifyMessageInChannel(ulong channelId, ulong messageId, Action<MessageProperties> func)
		{
			try
			{
				var channel = discord.GetChannel(channelId);
				if (channel is null)
				{
					return Results.ChannelNotFound;
				}
				if (channel is not IMessageChannel messageChannel)
				{
					return Results.ChannelNotText;
				}

				await messageChannel.ModifyMessageAsync(messageId, func);

				return Results.Success;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "{method}: Failed to modify message in channel: {ex}", nameof(ModifyMessageInChannel), ex);
				return Results.Failure;
			}
		}
	}
}
