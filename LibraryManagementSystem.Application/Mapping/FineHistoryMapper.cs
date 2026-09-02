using LibraryManagementSystem.Application.DTOs.Fine;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Mapping;

public static class FineHistoryMapper
{
	public static FineHistoryDto ToDto(this FineHistory history)
	{
		return new FineHistoryDto
		{
			Id = history.Id,
			FineId = history.FineId,
			LoanId = history.LoanId,
			UserId = history.UserId,
			UserName = $"{history.User.FirstName} {history.User.LastName}".Trim(),
			Action = history.Action,
			OccurredAt = history.OccurredAt,
			Description = history.Description
		};
	}
}