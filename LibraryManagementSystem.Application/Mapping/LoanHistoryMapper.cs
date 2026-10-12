using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.DTOs.Loans;

namespace LibraryManagementSystem.Application.Mapping;

public static class LoanHistoryMapper
{
	public static LoanHistoryDto ToDto(this LoanHistory history, string fullName, string title)
	{
		return new LoanHistoryDto
		{
			Id = history.Id,
			LoanId = history.LoanId,
			UserId = history.UserId,
			BookId = history.BookId,
			UserName = fullName.Trim(),
			BookName = title,
			Action = history.Action,
			OccurredAt = history.OccurredAt,
			Description = history.Description
		};
	}
}