using System.Text.Json.Serialization;
using MAOToolkit.Utilities.JsonConverters;

namespace MAOToolkit.Entities;

// https://github.com/omniti-labs/jsend
/// <summary>
/// Represents an API response without a typed payload.
/// </summary>
public class ApiResponse : ApiResponse<object> { }
/// <summary>
/// Represents an API response with a typed payload.
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// The result of the operation.
    /// </summary>
    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonCamelCaseEnumConverter<ApiResponseResult>))]
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

    /// <summary>
    /// Creates a successful API response.
    /// </summary>
    public static ApiResponse<T> Success(string message) => new() { Status = ApiResponseResult.Success, Message = message };

    /// <summary>
    /// Creates a successful API response.
    /// </summary>
    public static ApiResponse<T> Success(T data) => new() { Status = ApiResponseResult.Success, Data = data };

    /// <summary>
    /// Creates a successful API response.
    /// </summary>
    public static ApiResponse<T> Success(string message, T data) => new() { Status = ApiResponseResult.Success, Message = message, Data = data };

    /// <summary>
    /// Creates an error API response.
    /// </summary>
    public static ApiResponse<T> Error(string message) => new() { Status = ApiResponseResult.Error, Message = message };

    /// <summary>
    /// Creates an error API response.
    /// </summary>
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
