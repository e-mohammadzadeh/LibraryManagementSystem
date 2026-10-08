namespace LibraryManagementSystem.Infrastructure.Interfaces;

public interface IUserAutoRemovalService
{
	ServiceResult<string> TryAutoRemove(Guid userId);
}