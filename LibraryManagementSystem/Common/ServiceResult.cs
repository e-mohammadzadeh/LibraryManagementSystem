using LibraryManagementSystem.Domain;

namespace LibraryManagementSystem.Common;

public class ServiceResult<T>
{
	private ServiceResult(bool success, T data, string message)
	{
		Success = success;
		Data = data;
		Message = message;
	}

	public bool Success { get; private set; }
	public string Message { get; set; }
	public T? Data { get; set; }

	public static ServiceResult<T> ShowSuccessMessage(T data)
	{
		return new ServiceResult<T> { Success = true, Data = data, Message = "Operation completed successfully." };
	}

	public static ServiceResult<Book> ShowFailMessage(string message)
	{
		return new ServiceResult<Book> { Success = false, Data = null, Message = message };
	}
}