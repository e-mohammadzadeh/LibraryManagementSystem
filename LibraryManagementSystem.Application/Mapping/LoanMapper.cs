using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.DTOs.Loans;

namespace LibraryManagementSystem.Application.Mapping;

public static class LoanMapper
{
	public static LoanDto ToDto(this Loan loan)
	{
		return new LoanDto
		{
			LoanId = loan.LoanId,
			BookName = loan.Book.Title,
			BookId = loan.BookId,
			BookISBN = loan.Book.InternationalStandardBookNumber,
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