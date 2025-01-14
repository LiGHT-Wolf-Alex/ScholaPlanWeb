namespace ScholaPlan.API.DTOs
{
    /// <summary>
    /// Стандартизированный API-ответ.
    /// </summary>
    /// <typeparam name="T">Тип данных в ответе.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Успешно ли выполнен запрос.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Сообщение об ошибке или информации.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Данные ответа.
        /// </summary>
        public T? Data { get; set; }

        public ApiResponse(bool success, string message, T? data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static ApiResponse<T> SuccessResponse(string message, T data)
            => new ApiResponse<T>(true, message, data);

        public static ApiResponse<T> FailureResponse(string message)
            => new ApiResponse<T>(false, message);
    }
}