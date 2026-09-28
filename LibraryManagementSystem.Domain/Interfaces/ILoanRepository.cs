using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface ILoanRepository
{
	void Add(Loan loan);
	Loan? FindById(Guid id, LoanFilter filter = LoanFilter.Active);
	IReadOnlyList<Loan> GetLoans(LoanFilter filter = LoanFilter.Active);
	IReadOnlyList<Loan> GetAllByUser(Guid userId, LoanFilter filter = LoanFilter.Active);
	IReadOnlyList<Loan> GetLoansByBook(Guid bookId, LoanFilter filter = LoanFilter.Active);
	bool HasLoans(Guid? userId = null, Guid? bookId = null, LoanFilter filter = LoanFilter.Active);
	void Update(Loan loan);
	int CountLoans(Guid? userId = null, LoanFilter filter = LoanFilter.Active);
}