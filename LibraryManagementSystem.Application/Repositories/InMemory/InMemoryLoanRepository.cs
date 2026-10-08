using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryLoanRepository : ILoanRepository
{
	private readonly List<Loan> _loans = [];


	public void Add(Loan loan)
	{
		ArgumentNullException.ThrowIfNull(loan);

		loan.Id = Guid.CreateVersion7();
		loan.BorrowDate = DateOnly.FromDateTime(DateTime.UtcNow);
		loan.DueDate = loan.BorrowDate.AddDays(ValidationConstants.LoanPeriodDays);
		loan.ReturnDate = null;
		loan.Status = LoanStatus.Borrowed;
		loan.IsOverdue = false;
		loan.IsActive = true;
		loan.CreatedAt = DateTime.UtcNow;
		_loans.Add(loan);
	}


	public Loan? FindById(Guid id, LoanFilter filter = LoanFilter.Active)
	{
		return ApplyFilter(_loans, filter).FirstOrDefault(l => l.Id == id);
	}


	public IReadOnlyList<Loan> GetAll(LoanFilter filter = LoanFilter.Active)
	{
		return [.. ApplyFilter(_loans, filter)];
	}


	public IReadOnlyList<Loan> GetAllByUser(Guid userId, LoanFilter filter = LoanFilter.Active)
	{
		return [.. ApplyFilter(_loans, filter).Where(l => l.UserId == userId)];
	}


	public IReadOnlyList<Loan> GetLoansByBook(Guid bookId, LoanFilter filter = LoanFilter.Active)
	{
		return [.. ApplyFilter(_loans, filter).Where(l => l.BookId == bookId)];
	}


	public IReadOnlyList<Loan> GetLoansByBookAndUser(Guid bookId, Guid userId, LoanFilter filter = LoanFilter.All)
	{
		return [.. ApplyFilter(_loans, filter).Where(l => l.BookId == bookId && l.UserId == userId)];
	}


	public void Update(Loan loan)
	{
		// In-memory collections update by reference automatically.
		// However, we leave this method empty rather than throwing an exception 
		// so that the Service layer can safely call _repository.Update() 
		// without crashing, simulating a real database save operation.
	}


	// ---------- Private helper ----------
	private static IEnumerable<Loan> ApplyFilter(IEnumerable<Loan> source, LoanFilter filter)
	{
		return filter switch
		{
			LoanFilter.All => source,
			LoanFilter.Active => source.Where(l => l.IsActive),
			LoanFilter.Inactive => source.Where(l => !l.IsActive),
			LoanFilter.Overdue => source.Where(l => l.IsOverdue),
			LoanFilter.Returned => source.Where(l => l.ReturnDate.HasValue),
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
	}
}