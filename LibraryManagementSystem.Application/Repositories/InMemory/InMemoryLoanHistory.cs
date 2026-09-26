namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryLoanHistory : ILoanHistoryRepository
{
	private readonly List<LoanHistory> _histories = [];
	public void Add(LoanHistory history)
	{
		_histories.Add(history);
	}


	public IReadOnlyList<LoanHistory> GetByLoanId(Guid loanId)
	{
		return [.. _histories.Where(history => history.LoanId == loanId)];
	}


	public IReadOnlyList<LoanHistory> GetByUserId(Guid userId)
	{
		return [.. _histories.Where(history => history.UserId == userId)];
	}


	public IReadOnlyList<LoanHistory> GetByBookId(Guid bookId)
	{
		return [.. _histories.Where(history => history.BookId == bookId)];
	}


	public IReadOnlyList<LoanHistory> GetAll()
	{
		return _histories;
	}
}