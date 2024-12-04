using PokerTracker.Common.Models;

namespace PokerTracker.Common.Extensions
{
    public static class ResultExtensions
    {
        public static Result<T> Of<T>(this Result source, T value)
            => new(source.Code, source.Message, value, source.Success);

        public static Result<T?> Null<T>(this Result result)
            => new(result.Code, result.Message, default, result.Success);

        public static Result RemoveValue(this Result result)
            => new(result.Code, result.Message, result.Success);
    }
}
