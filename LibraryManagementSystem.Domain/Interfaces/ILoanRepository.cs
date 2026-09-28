using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface ILoanRepository
{
	void Add(Loan loan);
	Loan? FindById(Guid id, LoanFilter filter = LoanFilter.Active);
	IReadOnlyList<Loan> GetLoans(LoanFilter filter = LoanFilter.Active);
	IReadOnlyList<Loan> GetAllByUser(Guid userId);
	IReadOnlyList<Loan> GetLoansByUser(Guid userId, LoanFilter filter = LoanFilter.Active);
	IReadOnlyList<Loan> GetLoansByBook(Guid bookId, LoanFilter filter = LoanFilter.Active);
	bool HasLoans(Guid userId);
	void Update(Loan loan);


	int CountActiveLoansByUser(Guid userId);
	IReadOnlyList<Loan> GetLoansByBookAndUser(Guid bookId, Guid userId);
	int CountActiveLoans();
}