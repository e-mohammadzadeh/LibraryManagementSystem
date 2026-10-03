using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Interfaces;

public interface IFineHistoryManagementService
{
	void Record(Fine fine, FineHistoryAction action, string? description = null);
}