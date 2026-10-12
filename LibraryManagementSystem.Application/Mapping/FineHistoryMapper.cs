using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.DTOs.Fine;

namespace LibraryManagementSystem.Application.Mapping;

public static class FineHistoryMapper
{
	public static FineHistoryDto ToDto(this FineHistory history, string fullName)
	{
		return new FineHistoryDto
		{
			Id = history.Id,
			FineId = history.FineId,
			LoanId = history.LoanId,
			UserId = history.UserId,
			UserName = fullName.Trim(),
			Action = history.Action,
			OccurredAt = history.OccurredAt,
			Amount = history.Money,
			Description = history.Description
		};
	}
}