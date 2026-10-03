using LibraryManagementSystem.Application.Common;

namespace LibraryManagementSystem.Application.Interfaces;

public interface IUserAutoRemovalService
{
	ServiceResult<string> TryAutoRemove(Guid userId);
}