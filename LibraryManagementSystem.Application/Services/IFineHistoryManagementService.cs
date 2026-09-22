using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Application.Services;

public interface IFineHistoryManagementService
{
	void Record(Fine fine, FineHistoryAction action, string? description = null);
}