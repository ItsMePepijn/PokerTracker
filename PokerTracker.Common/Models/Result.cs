
namespace PokerTracker.Common.Models
{
	public record Result(string Code, string Message, bool Success = false);

	public record Result<T> : Result
	{
		public T Value { get; init; }

		public Result(string code, string message, T value, bool success = false)
			: base(code, message, success)
		{
			Value = value;
		}
	}
}
