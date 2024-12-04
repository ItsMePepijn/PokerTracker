
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PokerTracker.Service.Extensions;

namespace PokerTracker.Service.Services
{
	public class StartupService : IHostedService
	{
		private readonly DiscordSocketClient _discord;
		private readonly IConfiguration _config;
		private readonly ILogger<StartupService> _logger;

		public StartupService(
			DiscordSocketClient discord,
			IConfiguration config,
			ILogger<DiscordSocketClient> discordLogger,
			ILogger<StartupService> logger)
		{
			_discord = discord;
			_config = config;
			_logger = logger;

			_discord.Log += msg => msg.LogTo(discordLogger);
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			await _discord.LoginAsync(TokenType.Bot, _config["token"]);
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
