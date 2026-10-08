using LibraryManagementSystem.Infrastructure.Common;

namespace LibraryManagementSystem.Infrastructure.Interfaces;

public interface IUserAutoRemovalService
{
	ServiceResult<string> TryAutoRemove(Guid userId);
}