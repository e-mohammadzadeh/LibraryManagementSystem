using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Repositories.InMemory;

public class InMemoryFineRepository : IFineRepository
{
	private readonly List<Fine> _fines = [];


	public void Add(Fine fine)
	{
		ArgumentNullException.ThrowIfNull(fine);
		_fines.Add(fine);
	}


	public Fine? FindById(int fineId) { return _fines.FirstOrDefault(f => f.FineId == fineId); }


	public IReadOnlyList<Fine> GetAll() { return _fines.AsReadOnly(); }


	public IReadOnlyList<Fine> GetAllUnpaid()
	{
		return [.. _fines.Where(f => f.Status == FineStatus.Unpaid)];
	}


	public IReadOnlyList<Fine> GetByLoanId(int loanId)
	{
		return [.. _fines.Where(f => f.LoanId == loanId)];
	}


	public IReadOnlyList<Fine> GetByUserId(int userId)
	{
		return [.. _fines.Where(f => f.UserId == userId)];
	}


	public IReadOnlyList<Fine> GetUnpaidByUserId(int userId)
	{
		return [.. _fines.Where(f => f.UserId == userId && f.Status == FineStatus.Unpaid)];
	}


	public bool HasUnpaidFines(int userId)
	{
		return _fines.Any(f => f.UserId == userId && f.Status == FineStatus.Unpaid);
	}


	public decimal GetTotalUnpaidAmount(int userId)
	{
		return _fines.Where(f => f.UserId == userId && f.Status == FineStatus.Unpaid).Sum(f => f.Amount);
	}


	public void Update(Fine fine)
	{
		// In-memory implementation:
		// Fine is already tracked by reference.
	}
}