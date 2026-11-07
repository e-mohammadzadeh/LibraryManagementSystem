using LibraryManagementSystem.Infrastructure.DTOs.Loans;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Infrastructure.Interfaces;

public interface ILoanHistoryManagementService
{
	void Record(LoanDto loan, LoanHistoryAction action, string? description = null);
}