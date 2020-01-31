namespace ChiDaram.Common.Classes
{
    public class ApiResponse<T>
    {
        public ApiResponse()
        {
            Message = "";
        }

        /// <summary>
        /// اطلاعات خروجی
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// پیام
        /// </summary>
        public string Message { get; set; }

        public static ApiResponse<T> Success(T data, string message = "")
        {
            return new ApiResponse<T>
            {
                Message = message,
                Data = data
            };
        }
    }

    public class ApiResponse
    {
        /// <summary>
        /// پیام
        /// </summary>
        public string Message { get; set; }

        public static ApiResponse<bool> Success(string message = "")
        {
            return new ApiResponse<bool>
            {
                Message = message,
                Data = true
            };
        }
        public static ApiResponse<bool> Failure(string message = "")
        {
            return new ApiResponse<bool>
            {
                Message = message,
                Data = false
            };
        }
    }
}
