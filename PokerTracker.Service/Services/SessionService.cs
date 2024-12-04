using PokerTracker.Common.Models;
using PokerTracker.Persistence;
using PokerTracker.Persistence.Models;
using PokerTracker.Service.Interfaces;
using Results = PokerTracker.Common.Results;

namespace PokerTracker.Service.Services
{
	public class SessionService(
		Func<DataContext> contextFactory,
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

				context.Sessions.Add(session);
				await context.SaveChangesAsync();

				//TODO: send embed and update message id in db

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

				context.Sessions.RemoveRange(sessions);
				await context.SaveChangesAsync();

				//TODO remove embeds

				return Results.Success;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "{method}: Failed to clear sessions in channel: {ex}", nameof(ClearSessionsInChannel), ex);
				return Results.Failure;
			}
		}
	}
}
