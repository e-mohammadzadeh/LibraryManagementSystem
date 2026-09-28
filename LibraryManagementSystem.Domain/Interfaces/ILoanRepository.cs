using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface ILoanRepository
{
	void Add(Loan loan);
	Loan? FindById(Guid id, EntityFilter filter = EntityFilter.Active);
	IReadOnlyList<Loan> GetLoans(EntityFilter filter = EntityFilter.Active);
	IReadOnlyList<Loan> GetAllByUser(Guid userId);
	IReadOnlyList<Loan> GetLoansByUser(Guid userId, EntityFilter filter = EntityFilter.Active);
	IReadOnlyList<Loan> GetLoansByBook(Guid bookId, EntityFilter filter = EntityFilter.Active);
	IReadOnlyList<Loan> GetReturnedLoans();

	int CountActiveLoansByUser(Guid userId);
	bool HasActiveLoans(Guid userId, Guid bookId);
	bool HasOverdueLoans(Guid userId);
	IReadOnlyList<Loan> GetLoansByBookAndUser(Guid bookId, Guid userId);
	IReadOnlyList<Loan> GetOverdueLoans();
	void Update(Loan loan);
	int CountActiveLoans();
}