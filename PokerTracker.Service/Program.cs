using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PokerTracker.Service.Services;

using IHost host = Host.CreateDefaultBuilder(args)
	.ConfigureAppConfiguration(config =>
	{
		config.AddYamlFile("_config.yml", false);
	})
	.ConfigureServices(services =>
	{
		services.AddSingleton<DiscordSocketClient>();
		services.AddSingleton<InteractionService>();
		services.AddHostedService<StartupService>();
	})
	.Build();

await host.RunAsync();