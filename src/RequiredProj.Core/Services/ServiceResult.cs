namespace RequiredProj.Core.Services;

public class ServiceResult<T>
{
    public T? Value { get; }
    public string? Error { get; }
    public bool IsNotFound { get; }
    public bool IsSuccess => Error is null && !IsNotFound;

    private ServiceResult(T? value, string? error, bool isNotFound)
    {
        Value = value;
        Error = error;
        IsNotFound = isNotFound;
    }

    public static ServiceResult<T> Success(T value) => new(value, null, false);
    public static ServiceResult<T> NotFound() => new(default, null, true);
    public static ServiceResult<T> Failure(string error) => new(default, error, false);
}
