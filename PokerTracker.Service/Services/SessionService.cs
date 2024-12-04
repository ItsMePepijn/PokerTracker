using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using PokerTracker.Common.Extensions;
using PokerTracker.Common.Models;
using PokerTracker.Persistence;
using PokerTracker.Persistence.Models;
using PokerTracker.Service.Interfaces;
using Results = PokerTracker.Common.Results;

namespace PokerTracker.Service.Services
{
	public class SessionService(
		Func<DataContext> contextFactory,
		DiscordSocketClient discord,
		IMessageService messageService,
		ILogger<SessionService> logger) : ISessionService
	{
		public async Task<Result> CreateSession(ulong channelId, int startingBalance)
		{
			try
			{
				if (startingBalance <= 0)
				{
					return Results.InvalidBalance;
				}

				await using var context = contextFactory();

				var sessionsInChannel = context.Sessions.Count(s => s.ChannelId == channelId);
				if (sessionsInChannel > 0)
				{
					return Results.AlreadySessionInChannel;
				}

				var session = new Session
				{
					ChannelId = channelId,
					StartingBalance = startingBalance
				};

				await context.Sessions.AddAsync(session);
				await context.SaveChangesAsync();

				var embed = CreateEmbedForSession(session);
				var messageResult = await messageService.SendMessageToChannelAsync(channelId, embed: embed);

				if (!messageResult.Success)
				{
					context.Sessions.Remove(session);
					await context.SaveChangesAsync();
					return messageResult.RemoveValue();
				}

				session.ExistingEmbedMessageId = messageResult.Value?.Id;
				await context.SaveChangesAsync();

				return Results.Success;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "{method}: Failed to create session: {ex}", nameof(CreateSession), ex);
				return Results.Failure;
			}
		}

		public async Task<Result> ClearSessionsInChannel(ulong channelId)
		{
			try
			{
				await using var context = contextFactory();
				var sessions = context.Sessions.Where(s => s.ChannelId == channelId);

				foreach (var session in sessions)
				{
					if (session.ExistingEmbedMessageId.HasValue)
					{
						await messageService.DeleteMessageFromChannel(channelId, session.ExistingEmbedMessageId.Value);
					}
				}

				context.Sessions.RemoveRange(sessions);
				await context.SaveChangesAsync();

				return Results.Success;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "{method}: Failed to clear sessions in channel: {ex}", nameof(ClearSessionsInChannel), ex);
				return Results.Failure;
			}
		}

		public async Task<Result> SetBalanceForUserInChannel(ulong channelId, ulong userId, int balance)
		{
			try
			{
				await using var context = contextFactory();
				var session = context.Sessions
					.Include(s => s.Participants)
					.FirstOrDefault(s => s.ChannelId == channelId);
				if (session is null)
				{
					return Results.NoSessionInChannel;
				}

				var participant = session.Participants.FirstOrDefault(p => p.SessionId == session.Id && p.UserId == userId);
				if (participant is null)
				{
					participant = new Participant
					{
						SessionId = session.Id,
						UserId = userId,
						Balance = balance
					};
					session.Participants.Add(participant);
				}
				else
				{
					participant.Balance = balance;
				}

				await context.SaveChangesAsync();
				var embed = CreateEmbedForSession(session);

				var shouldCreate = false;
				if (session.ExistingEmbedMessageId is not null)
				{
					var messageResult = await messageService.ModifyMessageInChannel(channelId, session.ExistingEmbedMessageId.Value, m => m.Embed = embed);
					
					if(messageResult == Results.MessageNotFound)
					{
						shouldCreate = true;
					}
					else if (!messageResult.Success)
					{
						return messageResult.RemoveValue();
					}
				}

				if(session.ExistingEmbedMessageId is null || shouldCreate)
				{
					var messageResult = await messageService.SendMessageToChannelAsync(channelId, embed: embed);
					if (!messageResult.Success)
					{
						return messageResult.RemoveValue();
					}

					session.ExistingEmbedMessageId = messageResult.Value?.Id;
					await context.SaveChangesAsync();
				}

				return Results.Success;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "{method}: Failed to set balance for user in channel: {ex}", nameof(SetBalanceForUserInChannel), ex);
				return Results.Failure;
			}
		}

		private Embed CreateEmbedForSession(Session session)
		{
			var users = new List<SocketUser>();
			foreach (var participant in session.Participants)
			{
				var user = discord.GetUser(participant.UserId);
				if (user is not null)
				{
					users.Add(user);
				}
			}

			var usersWithBalance = session.Participants.Join(users, p => p.UserId, u => u.Id, (p, u) => new { User = u, p.Balance });

			var actualChipsVolume = session.Participants.Sum(p => p.Balance);
			var embedBuilder = new EmbedBuilder()
				.WithTitle("Session")
				.AddField("Starting Balance", $"`{session.StartingBalance}`")
				.AddField("Chip volume", $"`{session.StartingBalance * session.Participants.Count}`")
				.AddField("Actual chip volume", $"`{actualChipsVolume}`")
				.WithColor(new Color(0x00b9ff))
				.WithThumbnailUrl("https://cdn.discordapp.com/avatars/1313855807875452949/4e4bbdfb3662617bcebec167f1002d5e?size=1024")
				.WithCurrentTimestamp();

			if(usersWithBalance.Any())
			{
				embedBuilder.AddField("Participants", $"{string.Join("\n", usersWithBalance.Select(u => $"**{u.User.GlobalName}:** {u.Balance} chips ({GetDifferenceString(session.StartingBalance, u.Balance)}) [{GetVolumePercentileString(actualChipsVolume, u.Balance)}]"))}");
			}
			else
			{
				embedBuilder.AddField("Participants", "No participants yet");
			}

			return embedBuilder.Build();
		}

		private static string GetDifferenceString(int initialChipsValue, int userChipsValue)
		{
			var difference = userChipsValue - initialChipsValue;

			var differenceIndicator = difference switch
			{
				> 0 => "+",
				< 0 => "-",
				_ => ""
			};

			return $"{differenceIndicator}{Math.Abs(difference)}";
		}

		private static string GetVolumePercentileString(int chipsVolume, int userChipsValue)
		{
			var percentile = (double)userChipsValue / chipsVolume * 100;
			return $"{percentile:0.00}%";
		}
	}
}
