namespace LibraryManagementSystem.Common;

public class ServiceResult<T> where T : class
{
	private ServiceResult(bool success, T? data, string? message)
	{
		Success = success;
		Data = data;
		Message = message;
	}


	public bool Success { get; init; }
	public string? Message { get; init; }
	public T? Data { get; init; }


	public static ServiceResult<T> Ok(T data, string message)
	{
		return new ServiceResult<T>(true, data, message);
	}


	public static ServiceResult<T> Fail(string message)
	{
		return new ServiceResult<T>(false, null, message);
	}
}