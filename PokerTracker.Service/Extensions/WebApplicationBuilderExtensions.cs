
namespace PokerTracker.Service.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static void AddSetting<TSetting>(this WebApplicationBuilder builder) where TSetting : class
        {
            builder.Services.Configure<TSetting>(builder.Configuration.GetSection(typeof(TSetting).Name));
        }
    }
}
