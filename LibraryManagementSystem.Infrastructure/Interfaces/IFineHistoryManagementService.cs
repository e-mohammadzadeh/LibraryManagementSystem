using LibraryManagementSystem.Infrastructure.DTOs.Fine;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Infrastructure.Interfaces;

public interface IFineHistoryManagementService
{
	void Record(FineDto fine, FineHistoryAction action, string? description = null);
}