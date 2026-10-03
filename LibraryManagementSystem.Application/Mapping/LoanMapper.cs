using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.DTOs.Loans;

namespace LibraryManagementSystem.Application.Mapping;

public static class LoanMapper
{
	public static LoanDto ToDto(this Loan loan)
	{
		return new LoanDto
		{
			LoanId = loan.Id,
			BookName = loan.Book.Title,
			BookId = loan.BookId,
			BookISBN = loan.Book.ISBN,
			UserName = $"{loan.User.FirstName} {loan.User.LastName}",
			UserId = loan.UserId,
			UserNationalCode = loan.User.NationalCode,
			BorrowDate = loan.BorrowDate,
			DueDate = loan.DueDate,
			ReturnDate = loan.ReturnDate,
			Status = loan.Status,
			RenewalCount = loan.RenewalCount,
			IsOverdue = loan.IsOverdue,
			CreatedAt = loan.CreatedAt,
			UpdatedAt = loan.UpdatedAt
		};
	}
}