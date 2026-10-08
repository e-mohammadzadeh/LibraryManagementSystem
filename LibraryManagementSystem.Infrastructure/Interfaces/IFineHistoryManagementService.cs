using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Infrastructure.DTOs.Fine;

namespace LibraryManagementSystem.Infrastructure.Interfaces;

public interface IFineHistoryManagementService
{
	void Record(FineDto fine, FineHistoryAction action, string? description = null);
}