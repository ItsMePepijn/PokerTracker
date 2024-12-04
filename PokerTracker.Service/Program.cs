using Discord.Interactions;
using Discord.WebSocket;
using PokerTracker.Service.Extensions;
using PokerTracker.Service.Services;
using PokerTracker.Service.Settings;
using PokerTracker.Persistence;
using PokerTracker.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.AddSetting<DiscordClientSettings>();

builder.Services.AddTransient<ISessionService, SessionService>();
builder.Services.AddTransient<IMessageService, MessageService>();
builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
builder.Services.AddHostedService<StartupService>();
builder.Services.AddHostedService<InteractionHandlingService>();

builder.AddPersistence();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Services.EnsureDatabaseExists();

app.UseAuthorization();

app.MapControllers();

app.Run();
