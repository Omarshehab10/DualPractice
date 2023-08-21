namespace Common.Types
{
    public class ResultOfAction<T>  : BaseResult where T : class
    {
        public ResultOfAction(int statusCode, string responseMessage, T data)
        {
            StatusCode = statusCode;
            Message = responseMessage;
            Data = data;
        }
        public T Data { get; set; }
    }

    public class ResponseMessage
    {
        public ResponseMessage(string ar, string en)
        {
            Ar = ar;
            En = en;
        }
        public string Ar { get; set; }
        public string En { get; set; }
    }

    public class BaseResult
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
