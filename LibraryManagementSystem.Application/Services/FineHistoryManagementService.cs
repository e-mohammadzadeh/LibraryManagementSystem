using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Services;

public class FineHistoryManagementService
{
	public FineHistoryManagementService()
	{
		
	}


	public void Record(Fine fine, FineHistoryAction action, string? description = null)
	{
		var history = new FineHistory(fine, action, description);

	}
}