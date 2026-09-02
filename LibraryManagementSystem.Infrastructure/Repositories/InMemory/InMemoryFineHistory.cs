using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Repositories.InMemory;

public class InMemoryFineHistory : IFineHistoryRepository
{
	private readonly List<FineHistory> _histories = [];

	public void Add(FineHistory history)
	{
		_histories.Add(history);
	}


	public IReadOnlyList<FineHistory> GetByFineId(int fineId)
	{
		return [.. _histories.Where(history => history.FineId == fineId)];
	}


	public IReadOnlyList<FineHistory> GetByLoanId(int loanId)
	{
		return [.. _histories.Where(history => history.LoanId == loanId)];
	}


	public IReadOnlyList<FineHistory> GetByUserId(int userId)
	{
		return [.. _histories.Where(history => history.UserId == userId)];
	}


	public IReadOnlyList<FineHistory> GetAll()
	{
		return _histories;
	}
}