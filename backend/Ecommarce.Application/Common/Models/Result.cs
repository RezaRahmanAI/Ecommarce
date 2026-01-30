namespace Ecommarce.Application.Common.Models;

public class Result<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
    public string Message { get; set; } = string.Empty;

    public static Result<T> SuccessResult(T data, string message = "")
    {
        return new Result<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static Result<T> Failure(List<string> errors, string message = "Operation failed")
    {
        return new Result<T>
        {
            Success = false,
            Errors = errors,
            Message = message
        };
    }

    public static Result<T> Failure(string error, string message = "Operation failed")
    {
        return new Result<T>
        {
            Success = false,
            Errors = new List<string> { error },
            Message = message
        };
    }
}
