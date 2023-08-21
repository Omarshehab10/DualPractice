namespace DTO.DTOs.Responses
{
    public class ApiResponse<T>
    {
        /// <summary>
        /// Api Response
        /// </summary>
        public ApiResponse(T data = default, string errorMessage = "")
        {
            ErrorMessage = errorMessage;
            Data = data;
        }
        /// <summary>
        /// Error Message
        /// </summary>
        public string ErrorMessage { get; }
        /// <summary>
        /// Data
        /// </summary>
        public virtual T Data { get; }
    }
}
