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
			Action = history.Action,
			OccuredAt = history.OccurredAt,
			Description = history.Description
		};
	}
}