using System.Text.Json.Serialization;
using MAOToolkit.Utilities.JsonConverters;

namespace MAOToolkit.Entities
{
    // https://github.com/omniti-labs/jsend
    public class ApiResponse : ApiResponse<object> { }
    public class ApiResponse<T>
    {
        /// <summary>
        /// The result of the operation.
        /// </summary>
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonCamelCaseEnumConverter))]
        public ApiResponseResult Status { get; init; }

        /// <summary>
        /// Message about an error or a successful operation.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; init; } = String.Empty;

        /// <summary>
        /// Data obtained as a result of the operation.
        /// </summary>
        [JsonPropertyName("data")]
        public T? Data { get; init; }

        public static ApiResponse<T> Success(string message) => new() { Status = ApiResponseResult.Success, Message = message };

        public static ApiResponse<T> Success(T data) => new() { Status = ApiResponseResult.Success, Data = data };

        public static ApiResponse<T> Success(string message, T data) => new() { Status = ApiResponseResult.Success, Message = message, Data = data };

        public static ApiResponse<T> Error(string message) => new() { Status = ApiResponseResult.Error, Message = message };

        public static ApiResponse<T> Error(string message, T data) => new() { Status = ApiResponseResult.Error, Message = message, Data = data };
    }

    /// <summary>
    /// API response type.
    /// </summary>
    public enum ApiResponseResult
    {
        Success,
        Error
    }
}