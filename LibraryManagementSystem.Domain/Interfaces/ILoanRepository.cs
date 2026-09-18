using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface ILoanRepository
{
	void Add(Loan loan);
	Loan? FindById(Guid id);
	IReadOnlyList<Loan> GetAll();
	IReadOnlyList<Loan> GetAllByUser(Guid userId);
	Loan? GetActiveLoanById(Guid loanId);
	IReadOnlyList<Loan> GetActiveLoansByUser(Guid userId);
	IReadOnlyList<Loan> GetActiveLoansByBook(Guid bookId);
	IReadOnlyList<Loan> GetReturnedLoans();
	int CountActiveLoansByUser(Guid userId);
	bool HasActiveLoans(Guid userId, Guid bookId);
	bool HasOverdueLoans(Guid userId);
	IReadOnlyList<Loan> GetActiveLoans();
	IReadOnlyList<Loan> GetLoansByBook(Guid bookId);
	IReadOnlyList<Loan> GetLoansByBookAndUser(Guid bookId, Guid userId);
	IReadOnlyList<Loan> GetOverdueLoans();
	void Update(Loan loan);
	int CountActiveLoans();
}