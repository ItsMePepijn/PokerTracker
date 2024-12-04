using PokerTracker.Common.Models;

namespace PokerTracker.Service.Interfaces
{
    public interface ISessionService
    {
        Task<Result> CreateSession(ulong channelId, int startingBalance);
        Task<Result> ClearSessionsInChannel(ulong channelId);
        Task<Result> SetBalanceForUserInChannel(ulong channelId, ulong userId, int balance);
    }
}
