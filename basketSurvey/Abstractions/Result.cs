using static System.Runtime.InteropServices.JavaScript.JSType;

namespace basketSurvey.Abstractions
{
    public class Result
    {
        public Result(bool isSuccess ,Error error)
        {
            if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
                throw new InvalidOperationException();

            IsSuccess = isSuccess;
            Error = error;
        }
        public bool IsSuccess { get; }

        public Error Error { get; } = default!;

        public static Result Success() => new(true,Error.None);

        public static Result Failure(Error error) => new(false,error);

        public static Result<TValue> Success<TValue>(TValue value) => new(value,true, Error.None);

        public static Result<TValue> Failure<TValue>(Error error) => new(default, false,error);

    }

    public class Result<TValue> : Result
    {
        private TValue? _value { get; set; }
        public Result(TValue VALUE , bool isSuccess , Error error) : base( isSuccess,  error)
        {
            _value = VALUE;
        }

        public TValue? Value => IsSuccess ?
            _value :
            throw new InvalidOperationException("Failure Results Cannot have values");
    }
}
