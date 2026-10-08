using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface ILoanRepository
{
	void Add(Loan loan);
	Loan? FindById(Guid id, LoanFilter filter);
	IReadOnlyList<Loan> GetAll(LoanFilter filter);
	IReadOnlyList<Loan> GetAllByUser(Guid userId, LoanFilter filter);
	IReadOnlyList<Loan> GetLoansByBook(Guid bookId, LoanFilter filter);
	IReadOnlyList<Loan> GetLoansByBookAndUser(Guid bookId, Guid userId, LoanFilter filter);
	void Update(Loan loan);
}