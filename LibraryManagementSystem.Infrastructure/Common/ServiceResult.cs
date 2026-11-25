using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Infrastructure.Common;

public class ServiceResult<T> where T : class
{
	private ServiceResult(T? data, string? message, ResultStatus status)
	{
		Data = data;
		Message = message;
		Status = status;
	}


	public string? Message { get; init; }
	public T? Data { get; init; }
	public ResultStatus Status { get; init; }

	public bool IsSuccess => Status == ResultStatus.Success;
	public bool IsFailure => Status == ResultStatus.Failure;
	public bool IsWarning => Status == ResultStatus.Warning;


	public static ServiceResult<T> Ok(T data, string message)
	{
		ArgumentNullException.ThrowIfNull(data);
		return new ServiceResult<T>(data, message, ResultStatus.Success);
	}


	public static ServiceResult<T> Fail(string message)
	{
		return new ServiceResult<T>(null, message, ResultStatus.Failure);
	}


	public static ServiceResult<T> Warning(T data, string message)
	{
		return new ServiceResult<T>(data, message, ResultStatus.Warning);
	}
}