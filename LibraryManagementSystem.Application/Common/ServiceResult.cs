namespace LibraryManagementSystem.Common;

public class ServiceResult<T> where T : class
{
	private ServiceResult(bool success, T? data, string? message, ResultStatus status)
	{
		Success = success;
		Data = data;
		Message = message;
		Status = status;
	}


	public bool Success { get; init; }
	public string? Message { get; init; }
	public T? Data { get; init; }
	public ResultStatus Status { get; init; }


	public static ServiceResult<T> Ok(T data, string message)
	{
		return new ServiceResult<T>(true, data, message, ResultStatus.Success);
	}


	public static ServiceResult<T> Fail(string message)
	{
		return new ServiceResult<T>(false, null, message, ResultStatus.Failure);
	}


	public static ServiceResult<T> Warning(T data, string message)
	{
		return new ServiceResult<T>(true, data, message, ResultStatus.Warning);
	}
}