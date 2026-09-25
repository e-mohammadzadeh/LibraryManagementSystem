namespace LibraryManagementSystem.Infrastructure.Repositories.InMemory;

public class InMemoryFineHistory : IFineHistoryRepository
{
	private readonly List<FineHistory> _histories = [];

	public void Add(FineHistory history)
	{
		_histories.Add(history);
	}


	public IReadOnlyList<FineHistory> GetByFineId(Guid fineId)
	{
		return [.. _histories.Where(history => history.FineId == fineId)];
	}


	public IReadOnlyList<FineHistory> GetByLoanId(Guid loanId)
	{
		return [.. _histories.Where(history => history.LoanId == loanId)];
	}


	public IReadOnlyList<FineHistory> GetByUserId(Guid userId)
	{
		return [.. _histories.Where(history => history.UserId == userId)];
	}


	public IReadOnlyList<FineHistory> GetAll()
	{
		return _histories;
	}
}