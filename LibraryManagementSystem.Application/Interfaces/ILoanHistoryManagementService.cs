using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Application.Interfaces;

public interface ILoanHistoryManagementService
{
	void Record(Loan loan, LoanHistoryAction action, string? description = null);
}