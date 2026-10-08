using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Infrastructure.Interfaces;

public interface ILoanManagementService
{
	int CountLoans(Guid? userId, LoanFilter filter);
}