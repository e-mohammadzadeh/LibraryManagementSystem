using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface ILoanRepository
{
	void Add(Loan loan);
	Loan? GetById(Guid id, LoanFilter filter);
	IReadOnlyList<Loan> GetAll(LoanFilter filter);
	IReadOnlyList<Loan> GetByUserId(Guid userId, LoanFilter filter);
	IReadOnlyList<Loan> GetByBookId(Guid bookId, LoanFilter filter);
	IReadOnlyList<Loan> GetLoansByBookAndUser(Guid bookId, Guid userId, LoanFilter filter);
	void Update(Loan loan);
}