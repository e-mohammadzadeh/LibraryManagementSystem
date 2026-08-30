using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface ILoanHistoryRepository
{
	void Add(LoanHistory history);
	IReadOnlyList<LoanHistory> GetByLoanId(int loanId);
	IReadOnlyList<LoanHistory> GetByUserId(int userId);
	IReadOnlyList<LoanHistory> GetByBookId(int bookId);
	IReadOnlyList<LoanHistory> GetAll();
}