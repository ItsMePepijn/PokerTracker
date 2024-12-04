
namespace PokerTracker.Persistence.Models
{
    public abstract class Entity
    {
        public Guid Id { get; init; }
        public DateTimeOffset CreatedAt { get; internal set; }
    }
}
