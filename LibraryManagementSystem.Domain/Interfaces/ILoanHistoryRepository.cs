using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface ILoanHistoryRepository
{
	void Add(LoanHistory history);
	IReadOnlyList<LoanHistory> GetByLoan(int loanId);
	IReadOnlyList<LoanHistory> GetByUser(int userId);
	IReadOnlyList<LoanHistory> GetByBook(int bookId);
	IReadOnlyList<LoanHistory> GetAll();
}