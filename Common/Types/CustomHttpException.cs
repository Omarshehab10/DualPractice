using System;
using System.Net;

namespace Common.Types
{
    public class CustomHttpException : Exception
    {
        public int StatusCode { get; set; }
        public new string Message { get; set; }
        public CustomHttpException(HttpStatusCode httpStatusCode, string ResponseMessage)
        {
            StatusCode = (int)httpStatusCode;
            Message = ResponseMessage;
        }
        public CustomHttpException(int httpStatusCode, string ResponseMessage)
        {
            StatusCode = httpStatusCode;
            Message = ResponseMessage;
        }
    }
}
