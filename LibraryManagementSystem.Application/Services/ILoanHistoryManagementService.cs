using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Services;

public interface ILoanHistoryManagementService
{
	void Record(Loan loan, LoanHistoryAction action, string? description = null);
}