using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Repositories.InMemory;

public class InMemoryLoanRepository : ILoanRepository
{
	private readonly List<Loan> _loans = [];


	public void Add(Loan loan)
	{
		_loans.Add(loan);
	}


	public Loan? FindById(int id)
	{
		return _loans.FirstOrDefault(l => l.LoanId == id);
	}


	public IReadOnlyList<Loan> GetAll()
	{
		return _loans.AsReadOnly();
	}


	public IReadOnlyList<Loan> GetAllByUser(int userId)
	{
		return _loans.Where(l => l.UserId == userId).ToList().AsReadOnly();
	}


	public Loan? GetActiveLoanById(int loanId)
	{
		return _loans.FirstOrDefault(l => l.LoanId == loanId && l.IsActive);
	}



	public IReadOnlyList<Loan> GetActiveLoansByUser(int userId)
	{
		return [.. _loans.Where(l => l.UserId == userId && l.ReturnDate == null)];
	}


	public int CountActiveLoansByUser(int userId)
	{
		return _loans.Count(l => l.UserId == userId && l.ReturnDate == null);
	}


	public bool HasActiveLoan(int userId, int bookId)
	{
		return _loans.Any(l => l.UserId == userId && l.BookId == bookId && l.ReturnDate == null);
	}


	public Loan? GetActiveLoan(int userId, int bookId)
	{
		return _loans.FirstOrDefault(l => l.UserId == userId && l.BookId == bookId && l.ReturnDate == null);
	}


	public IReadOnlyList<Loan> GetActiveLoans()
	{
		return _loans.Where(l => l.ReturnDate == null).ToList().AsReadOnly();
	}


	public IReadOnlyList<Loan> GetLoansByBook(int bookId)
	{
		return [.. _loans.Where(l => l.BookId == bookId)];
	}


	public IReadOnlyList<Loan> GetReturnedLoans()
	{
		return [.. _loans.Where(l => l.Status == LoanStatus.Returned)];
	}


	public IReadOnlyList<Loan> GetOverdueLoans()
	{
		return [.. _loans.Where(l => l.IsOverdue)];
	}


	public void Remove(Loan loan)
	{
		_loans.Remove(loan);
	}


	public void Update(Loan loan)
	{
		// In-memory collections update by reference automatically.
		// However, we leave this method empty rather than throwing an exception 
		// so that the Service layer can safely call _repository.Update() 
		// without crashing, simulating a real database save operation.
	}


	public int CountActiveLoans()
	{
		return _loans.Count(l=>l.IsActive);
	}
}