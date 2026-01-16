namespace INVEST.Application.Common.Errors
{
    public class Result
    {
        public bool Success { get; }
        public IReadOnlyList<Error> Errors { get; }

        protected Result(bool success, IReadOnlyList<Error> errors)
            => (Success, Errors) = (success, errors);

        public static Result Ok() => new(true, Array.Empty<Error>());
        public static Result Fail(params Error[] errors) => new(false, errors);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(bool success, T? value, IReadOnlyList<Error> errors) : base(success, errors)
            => Value = value;

        public static Result<T> Ok(T value) => new(true, value, Array.Empty<Error>());
        public static new Result<T> Fail(params Error[] errors) => new(false, default, errors);
    }
}
