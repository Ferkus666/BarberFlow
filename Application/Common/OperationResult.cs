namespace BarberFlow.Application.Common;

public class OperationResult
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public IReadOnlyCollection<string> Errors { get; init; } = Array.Empty<string>();

    public static OperationResult Ok(string? message = null) => new() { Success = true, Message = message };
    public static OperationResult Fail(string error) => new() { Success = false, Errors = new[] { error } };
    public static OperationResult Fail(IEnumerable<string> errors) => new() { Success = false, Errors = errors.ToArray() };
}

public sealed class OperationResult<T> : OperationResult
{
    public T? Data { get; init; }

    public static OperationResult<T> Ok(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    public new static OperationResult<T> Fail(string error) =>
        new() { Success = false, Errors = new[] { error } };
}
