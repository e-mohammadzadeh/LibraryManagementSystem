using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface ILoanRepository
{
	void Add(Loan loan);
	Loan? FindById(int id);
	IReadOnlyList<Loan> GetAll();
	void GetBorrowedBooks();
	IReadOnlyList<Loan> GetByUser(int userId);
	IReadOnlyList<Loan> GetByBook();
	void Remove(Loan loan);
	void GetOverdueLoans();
	void GetReturnedLoans();
}