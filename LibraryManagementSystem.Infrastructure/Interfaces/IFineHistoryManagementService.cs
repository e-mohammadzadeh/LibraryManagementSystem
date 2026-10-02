using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Infrastructure.Interfaces;

public interface IFineHistoryManagementService
{
	void Record(Fine fine, FineHistoryAction action, string? description = null);
}