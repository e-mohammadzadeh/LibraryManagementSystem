using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Infrastructure.Interfaces;

public interface ILoanHistoryManagementService
{
	void Record(Loan loan, LoanHistoryAction action, string? description = null);
}