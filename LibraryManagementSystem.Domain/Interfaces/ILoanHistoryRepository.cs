using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface ILoanHistoryRepository
{
	void Add(LoanHistory history);
	IReadOnlyList<LoanHistory> GetByLoanId(Guid loanId);
	IReadOnlyList<LoanHistory> GetByUserId(Guid userId);
	IReadOnlyList<LoanHistory> GetByBookId(Guid bookId);
	IReadOnlyList<LoanHistory> GetAll();
}