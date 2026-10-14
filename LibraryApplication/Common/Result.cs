
namespace LibraryApi.Common
{
    public class Result
    {
        public bool IsSucess { get; }
        public IReadOnlyList<Errors> Errors { get; }

        protected Result(bool issucess, IReadOnlyList<Errors> errors)
        {
            IsSucess = issucess;
            Errors = errors;
        }

        public static Result Ok() => new Result(true, Array.Empty<Errors>());
        public static Result Fail(Errors error) => new Result(false, new[] { error });
        public static Result Fail(IReadOnlyList<Errors> errors) => new Result(false, errors);
    }

    public class Result<Tvalue> : Result
    {
        private readonly Tvalue _value;
        public Tvalue data => IsSucess ? _value : throw new InvalidOperationException("Can Not Access Value Of Failed Result");
        private Result(Tvalue value) : base(true, Array.Empty<Errors>())
        {
            _value = value;
        }

        private Result(Errors error) : base(false, new[] { error })
        {
            _value = default!;
        }

        private Result(IReadOnlyList<Errors> errors) : base(false, errors)
        {
            _value = default!;
        }

        public static Result<Tvalue> Ok(Tvalue value) => new Result<Tvalue>(value);
        public static Result<Tvalue> Fail(Errors error) => new Result<Tvalue>(error);
        public static Result<Tvalue> Fail(IReadOnlyList<Errors> errors) => new Result<Tvalue>(errors);


        public static implicit operator Result<Tvalue>(Tvalue value) => Ok(value);
        public static implicit operator Result<Tvalue>(Errors error) => Fail(error);
    }
}
