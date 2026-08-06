using LibraryManagementSystem.Application.Common;

namespace LibraryManagementSystem.Application.Services;

public interface IUserAutoRemovalService
{
	bool CanBeAutoRemoved(int userId);
	ServiceResult<string> TryAutoRemove(int userId);
}