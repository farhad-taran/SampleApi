namespace Books.Api.Contracts.Common
{
    public class Error
    {
        public Error(string errorCode, string errorMessage)
        {
            Code = errorCode;
            Message = errorMessage;
        }

        public string Code { get; set; }

        public string Message { get; set; }
    }
}