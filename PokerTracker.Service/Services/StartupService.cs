using Discord.WebSocket;
using Discord;
using PokerTracker.Service.Extensions;
using PokerTracker.Service.Settings;
using Microsoft.Extensions.Options;

namespace PokerTracker.Service.Services
{
	public class StartupService : IHostedService
	{
		private readonly DiscordSocketClient _discord;
		private readonly DiscordClientSettings _clientSettings;
		private readonly ILogger<StartupService> _logger;

		public StartupService(
			DiscordSocketClient discord,
			IOptions<DiscordClientSettings> clientSettingss,
			ILogger<DiscordSocketClient> discordLogger,
			ILogger<StartupService> logger)
		{
			_discord = discord;
			_clientSettings = clientSettingss.Value;
			_logger = logger;

			_discord.Log += msg => msg.LogTo(discordLogger);
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			await _discord.LoginAsync(TokenType.Bot, _clientSettings.Token);
			await _discord.StartAsync();
			_logger.LogInformation("Bot started at {currenttime}", DateTime.Now.ToString("dd-MM-yyyy HH:mm"));

		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			await _discord.LogoutAsync();
			await _discord.StopAsync();
			_logger.LogInformation("Bot stopped at {currenttime}", DateTime.Now.ToString("dd-MM-yyyy HH:mm"));
		}
	}
}
