using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PokerTracker.Persistence
{
    public static class DependencyInjection
    {
        public static IHostApplicationBuilder AddPersistence(this IHostApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetSection(nameof(PersistenceSettings)).Get<PersistenceSettings>()?.ConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException($"{nameof(PersistenceSettings)}:{nameof(PersistenceSettings.ConnectionString)} not found in configuration.");

            builder.Services.AddDbContextFactory<DataContext>(options =>
            {
                options.UseSqlite(connectionString);
            });

            builder.Services.AddTransient<Func<DataContext>>(provider => () => provider.GetService<IDbContextFactory<DataContext>>()!.CreateDbContext());

            return builder;
        }
    }
}
