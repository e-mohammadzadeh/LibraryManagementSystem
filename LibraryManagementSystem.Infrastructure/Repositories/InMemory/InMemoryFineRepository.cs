using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Repositories.InMemory;

public class InMemoryFineRepository:IFineRepository
{
	private readonly List<Fine> _fines = [];

	public void Add(Fine fine)
	{
		_fines.Add(fine);
	}


	public Fine? FindById(int findId)
	{
		return _fines.FirstOrDefault(f => f.FineId == findId);
	}


	public IReadOnlyList<Fine> GetAll()
	{
		return _fines.ToList().AsReadOnly();
	}


	public IReadOnlyList<Fine> GetByUserId(int userId)
	{
		return _fines.Where(f => f.Loan.UserId == userId).ToList().AsReadOnly();
	}


	public IReadOnlyList<Fine> GetUnpaidByUserId(int userId)
	{
		return _fines.Where(f => f.Loan.UserId == userId && f.Status == FineStatus.Unpaid).ToList().AsReadOnly();
	}


	public IReadOnlyList<Fine> GetUnpaidByLoanId(int loanId)
	{
		return _fines.Where(f => f.LoanId == loanId && f.Status == FineStatus.Unpaid).ToList().AsReadOnly();
	}


	public bool HasUnpaidFines(int userId)
	{
		return _fines.Any(f => f.Loan.UserId == userId && f.Status == FineStatus.Unpaid);
	}


	public decimal GetTotalUnpaidAmound(int userId)
	{
		return _fines.Where(f => f.Loan.UserId == userId && f.Status == FineStatus.Unpaid).Sum(f => f.Amount);
	}


	public void Remove(Fine fine)
	{
		_fines.Remove(fine);
	}


	public void Update(Fine fine)
	{
		// In-memory implementation:
		// Fine is already tracked by reference.
	}


	public int Count()
	{
		return _fines.Count;
	}
}