using Discord;
using PokerTracker.Common.Models;

namespace PokerTracker.Service.Interfaces
{
    public interface IMessageService
    {
        Task<Result<IUserMessage?>> SendMessageToChannelAsync(
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
            PollProperties? poll = null);

        Task<Result> DeleteMessageFromChannel(ulong channelId, ulong messageId);
        Task<Result> ModifyMessageInChannel(ulong channelId, ulong messageId, Action<MessageProperties> func);
    }
}
