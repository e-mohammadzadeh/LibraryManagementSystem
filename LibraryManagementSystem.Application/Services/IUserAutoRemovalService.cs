using LibraryManagementSystem.Application.Common;

namespace LibraryManagementSystem.Application.Services;

public interface IUserAutoRemovalService
{
	ServiceResult<string> TryAutoRemove(int userId);
}