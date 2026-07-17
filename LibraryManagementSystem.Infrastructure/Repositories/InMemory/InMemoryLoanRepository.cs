using LibraryManagementSystem.Domain.Entities;
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



	public IReadOnlyList<Loan> GetActiveLoansByUser(int userId)
	{
		return _loans.Where(l => l.UserId == userId && l.ReturnDate == null).ToList();
	}


	public int CountActiveLoansByUser(int userId)
	{
		return GetActiveLoansByUser(userId).Count;
	}


	public bool HasActiveLoan(int userId, int bookId)
	{
		return _loans.Any(l => l.UserId == userId && l.BookId == bookId && l.ReturnDate == null);
	}


	public Loan? GetActiveLoan(int userId, int bookId)
	{
		return _loans.FirstOrDefault(l => l.UserId == userId && l.BookId == bookId && l.ReturnDate == null);
	}


	public IReadOnlyList<Loan> GetLoansByBook(int bookId)
	{
		throw new NotImplementedException();
	}


	IReadOnlyList<Loan> ILoanRepository.GetReturnedLoans()
	{
		throw new NotImplementedException();
	}


	IReadOnlyList<Loan> ILoanRepository.GetOverdueLoans()
	{
		throw new NotImplementedException();
	}


	public void Remove(Loan loan)
	{
		_loans.Remove(loan);
	}


	public void Update(Loan loan)
	{
		throw new NotImplementedException();
	}
}