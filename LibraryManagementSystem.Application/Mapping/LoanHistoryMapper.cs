using LibraryManagementSystem.Application.DTOs.Loans;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Mapping;

public static class LoanHistoryMapper
{
	public static LoanHistoryDto ToDto(this LoanHistory history, string userName, string bookName)
	{
		return new LoanHistoryDto
		{
			Id = history.Id,
			LoanId = history.LoanId,
			UserId = history.UserId,
			BookId = history.BookId,
			UserName = userName,
			BookName = bookName,
			Action = history.Action,
			OccurredAt = history.OccurredAt,
			Description = history.Description
		};
	}
}