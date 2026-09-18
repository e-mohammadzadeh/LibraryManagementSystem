using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryLoanRepository : ILoanRepository
{
	private readonly List<Loan> _loans = [];


	public void Add(Loan loan)
	{
		ArgumentNullException.ThrowIfNull(loan);
		_loans.Add(loan);
	}


	public Loan? FindById(Guid id) { return _loans.FirstOrDefault(l => l.LoanId == id); }


	public IReadOnlyList<Loan> GetAll() { return _loans.AsReadOnly(); }


	public IReadOnlyList<Loan> GetAllByUser(Guid userId) { return [.. _loans.Where(l => l.UserId == userId)]; }


	public Loan? GetActiveLoanById(Guid loanId) { return _loans.FirstOrDefault(l => l.LoanId == loanId && l.IsActive); }


	public IReadOnlyList<Loan> GetActiveLoansByUser(Guid userId)
	{
		return [.. _loans.Where(l => l.UserId == userId && l.IsActive)];
	}


	public IReadOnlyList<Loan> GetActiveLoansByBook(Guid bookId)
	{
		return [.. _loans.Where(l => l.BookId == bookId && l.IsActive)];
	}


	public IReadOnlyList<Loan> GetReturnedLoans() { return [.. _loans.Where(loan => loan.ReturnDate.HasValue)]; }


	public int CountActiveLoansByUser(Guid userId) { return _loans.Count(l => l.UserId == userId && l.IsActive); }


	public bool HasActiveLoans(Guid userId, Guid bookId)
	{
		return _loans.Any(l => l.UserId == userId && l.BookId == bookId && l.IsActive);
	}


	public bool HasOverdueLoans(Guid userId)
	{
		return _loans.Any(l => l.UserId == userId && l is { IsActive: true, IsOverdue: true });
	}


	public IReadOnlyList<Loan> GetActiveLoans() { return [.. _loans.Where(l => l.IsActive)]; }


	public IReadOnlyList<Loan> GetLoansByBook(Guid bookId) { return [.. _loans.Where(l => l.BookId == bookId)]; }


	public IReadOnlyList<Loan> GetLoansByBookAndUser(Guid bookId, Guid userId)
	{
		return [.. _loans.Where(l => l.BookId == bookId && l.UserId == userId)];
	}


	public IReadOnlyList<Loan> GetOverdueLoans() { return [.. _loans.Where(l => l.IsOverdue)]; }


	public void Update(Loan loan)
	{
		// In-memory collections update by reference automatically.
		// However, we leave this method empty rather than throwing an exception 
		// so that the Service layer can safely call _repository.Update() 
		// without crashing, simulating a real database save operation.
	}


	public int CountActiveLoans() { return _loans.Count(l => l.IsActive); }
}