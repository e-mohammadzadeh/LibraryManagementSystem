using LibraryManagementSystem.Application.DTOs.Fine;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Application.Mapping;

public static class FineMapper
{
	public static FineDto ToDto(this Fine fine)
	{
		return new FineDto
		{
			FineId = fine.FineId,
			LoanId = fine.LoanId,
			UserId = fine.UserId,
			UserFullName = fine.Loan?.User is not null
				? $"{fine.Loan.User.FirstName} {fine.Loan.User.LastName}"
				: "Unknown",
			BookName = fine.Loan?.Book?.BookName ?? "Unknown",
			OverdueDays = fine.OverdueDays,
			Amount = fine.Amount,
			Status = fine.Status,
			Reason = fine.Reason,
			DailyRate = fine.DailyRate,
			CreatedAt = fine.CreatedAt,
			UpdatedAt = fine.UpdatedAt,
			PaidAt = fine.PaidAt
		};
	}
}