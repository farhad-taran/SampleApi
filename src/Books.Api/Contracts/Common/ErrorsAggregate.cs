namespace Books.Api.Contracts.Common
{
    public class ErrorsAggregate
    {
        public string MainErrorCode { get; }
        public Error[] Errors { get; }

        public ErrorsAggregate(string mainErrorCode, Error[] errors)
        {
            MainErrorCode = mainErrorCode;
            Errors = errors;
        }

        public ErrorsAggregate(Error error) : this(error.Code, new[] { error })
        {
        }
    }
}