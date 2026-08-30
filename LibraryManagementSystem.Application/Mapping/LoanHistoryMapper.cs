using LibraryManagementSystem.Application.DTOs.Loans;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Mapping;

public static class LoanHistoryMapper
{
	public static LoanHistoryDto ToDto(this LoanHistory history)
	{
		return new LoanHistoryDto
		{
			Id = history.Id,
			LoanId = history.LoanId,
			UserId = history.UserId,
			BookId = history.BookId,
			UserName = $"{history.User.FirstName} {history.User.LastName}".Trim(),
			BookName = history.Book.BookName,
			Action = history.Action,
			OccurredAt = history.OccurredAt,
			Description = history.Description
		};
	}
}