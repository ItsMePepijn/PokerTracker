using Microsoft.EntityFrameworkCore;
using PokerTracker.Persistence.Models;

namespace PokerTracker.Persistence
{
	public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
	{

		public override int SaveChanges()
		{
			UpdateEntityTimestamps();
			return base.SaveChanges();
		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			UpdateEntityTimestamps();
			return await base.SaveChangesAsync(cancellationToken);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}

		private void UpdateEntityTimestamps()
		{
			var entries = ChangeTracker.Entries<Entity>();

			foreach (var entry in entries)
			{
				if (entry.State == EntityState.Added)
				{
					entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
				}
			}
		}
	}
}
