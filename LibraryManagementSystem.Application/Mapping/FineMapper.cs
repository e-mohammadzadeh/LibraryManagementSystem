using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.DTOs.Fine;

namespace LibraryManagementSystem.Application.Mapping;

public static class FineMapper
{
	public static FineDto ToDto(this Fine fine)
	{
		return new FineDto
		{
			FineId = fine.Id,
			LoanId = fine.LoanId,
			UserId = fine.UserId,
			UserFullName = $"{fine.Loan.User.FirstName} {fine.Loan.User.LastName}",
			BookName = fine.Loan.Book.Title,
			OverdueDays = fine.OverdueDays,
			Amount = fine.Money,
			Status = fine.Status,
			Reason = fine.Reason,
			CreatedAt = fine.CreatedAt,
			UpdatedAt = fine.UpdatedAt,
			PaidAt = fine.PaidAt
		};
	}
}