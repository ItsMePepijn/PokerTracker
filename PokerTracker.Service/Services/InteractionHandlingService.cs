using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using System.Reflection;
using PokerTracker.Service.Extensions;

namespace PokerTracker.Service.Services
{
    public class InteractionHandlingService : IHostedService
    {
        private readonly DiscordSocketClient _discord;
        private readonly InteractionService _interactions;
        private readonly IServiceProvider _services;

        public InteractionHandlingService(
            DiscordSocketClient discord,
            InteractionService interactions,
            IServiceProvider services,
            ILogger<InteractionService> logger)
        {
            _discord = discord;
            _interactions = interactions;
            _services = services;

            _interactions.Log += msg => msg.LogTo(logger);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _discord.Ready += async () =>
            {
#if DEBUG
                await _interactions.RegisterCommandsToGuildAsync(1027991260981633035, true);
#endif
                await _interactions.RegisterCommandsGloballyAsync(true);
            };
            _discord.InteractionCreated += OnInteractionAsync;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _interactions.Dispose();
            return Task.CompletedTask;
        }

        private async Task OnInteractionAsync(SocketInteraction interaction)
        {
            try
            {
                var context = new SocketInteractionContext(_discord, interaction);
                var result = await _interactions.ExecuteCommandAsync(context, _services);

                if (!result.IsSuccess)
                    await context.Channel.SendMessageAsync(result.ToString());
            }
            catch
            {
                if (interaction.Type == InteractionType.ApplicationCommand)
                {
                    await interaction.GetOriginalResponseAsync()
                        .ContinueWith(msg => msg.Result.DeleteAsync());
                }
            }
        }
    }
}
