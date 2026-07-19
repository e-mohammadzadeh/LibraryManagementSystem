using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface ILoanRepository
{
	void Add(Loan loan);
	Loan? FindById(int id);
	IReadOnlyList<Loan> GetAll();
	IReadOnlyList<Loan> GetAllByUser(int userId);
	Loan? GetActiveLoanById(int loanId);
	IReadOnlyList<Loan> GetActiveLoansByUser(int userId);
	int CountActiveLoansByUser(int userId);
	bool HasActiveLoan(int userId, int bookId);
	Loan? GetActiveLoan(int userId, int bookId);
	IReadOnlyList<Loan> GetActiveLoans();
	IReadOnlyList<Loan> GetLoansByBook(int bookId);
	IReadOnlyList<Loan> GetReturnedLoans();
	IReadOnlyList<Loan> GetOverdueLoans();
	void Remove(Loan loan);
	void Update(Loan loan);
	int CountActiveLoans();

}