using PokerTracker.Common.Models;

namespace PokerTracker.Service.Interfaces
{
	public interface ISessionService
	{
		Task<Result> CreateSession(ulong channelId, int startingBalance);
		Task<Result> ClearSessionsInChannel(ulong channelId);
	}
}
