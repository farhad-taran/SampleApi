
using System;

namespace Books.Api.Contracts.Common
{
    public class ErrorResponse
    {
        public Error[] Errors { get; set; }

        public string CorrelationId { get; set; }

        public DateTimeOffset TimeStamp { get; set; }

        public string PathCalled { get; set; }

        public string ErrorCode { get; set; }
    }
}