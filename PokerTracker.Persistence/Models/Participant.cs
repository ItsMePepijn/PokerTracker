
namespace PokerTracker.Persistence.Models
{
    public class Participant : Entity
    {
        public ulong UserId { get; set; }
        public int Balance { get; set; }
        public Guid SessionId { get; set; }
        public Session? Session { get; set; }
    }
}
