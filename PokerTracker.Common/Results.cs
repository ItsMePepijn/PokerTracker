using PokerTracker.Common.Models;

namespace PokerTracker.Common
{
	public static class Results
	{
		public static readonly Result Success = new("A01", "Success!", true);
		public static readonly Result Failure = new("A02", "Something went wrong.");

		public static readonly Result InvalidBalance = new("A03", "Starting balance must be greater than 0.");
		public static readonly Result AlreadySessionInChannel = new("A04", "There is already a session in this channel.");
	}
}
