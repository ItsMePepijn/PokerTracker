
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace PokerTracker.Persistence
{
    public static class Seeder
    {
        public static void EnsureDatabaseExists(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var contextFactory = scope.ServiceProvider.GetRequiredService<Func<DataContext>>();

            var context = contextFactory();

            context.Database.Migrate();
        }
    }
}
