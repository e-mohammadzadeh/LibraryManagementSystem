using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Repositories.InMemory;

public class InMemoryLoanHistory : ILoanHistoryRepository
{
	private readonly List<LoanHistory> _histories = [];
	public void Add(LoanHistory history)
	{
		_histories.Add(history);
	}


	public IReadOnlyList<LoanHistory> GetByLoanId(int loanId)
	{
		return [.. _histories.Where(history => history.LoanId == loanId)];
	}


	public IReadOnlyList<LoanHistory> GetByUserId(int userId)
	{
		return [.. _histories.Where(history => history.UserId == userId)];
	}


	public IReadOnlyList<LoanHistory> GetByBookId(int bookId)
	{
		return [.. _histories.Where(history => history.BookId == bookId)];
	}


	public IReadOnlyList<LoanHistory> GetAll()
	{
		return _histories;
	}
}