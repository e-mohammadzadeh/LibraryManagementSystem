using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface ILoanRepository
{
	void Add(Loan loan);
	Loan? FindById(int id);
	IReadOnlyList<Loan> GetAll();
	IReadOnlyList<Loan> GetActiveLoansByUser(int userId);
	bool HasActiveLoan(int userId, int bookId);
	Loan? GetActiveLoan(int userId, int bookId);
	IReadOnlyList<Loan> GetLoansByBook(int bookId);
	IReadOnlyList<Loan> GetReturnedLoans();
	IReadOnlyList<Loan> GetOverdueLoans();
	void Remove(Loan loan);

}