
namespace PokerTracker.Persistence.Models
{
    public class Session : Entity
    {
        public ulong ChannelId { get; set; }
        public ulong? ExistingEmbedMessageId { get; set; }
        public int StartingBalance { get; set; }
        public List<Participant> Participants { get; set; } = [];
    }
}
