using Discord;
using Microsoft.Extensions.Logging;

namespace PokerTracker.Service.Extensions
{
	public static class LogMessageExtensions
	{
		public static Task LogTo(this LogMessage msg, ILogger logger)
		{
			switch (msg.Severity)
			{
				case LogSeverity.Verbose:
					logger.LogInformation(msg.ToString());
					break;

				case LogSeverity.Info:
					logger.LogInformation(msg.ToString());
					break;

				case LogSeverity.Warning:
					logger.LogWarning(msg.ToString());
					break;

				case LogSeverity.Error:
					logger.LogError(msg.ToString());
					break;

				case LogSeverity.Critical:
					logger.LogCritical(msg.ToString());
					break;
			}

			return Task.CompletedTask;
		}
	}
}
