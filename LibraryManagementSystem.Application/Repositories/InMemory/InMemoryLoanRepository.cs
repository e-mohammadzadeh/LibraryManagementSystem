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


	public bool HasLoans(Guid? userId = null, Guid? bookId = null, LoanFilter filter = LoanFilter.Active)
	{
		if (userId is null && bookId is null)
			throw new ArgumentException("At least one of userId or bookId must be provided.");
		var query = ApplyFilter(_loans, filter);

		if (userId is not null) query = query.Where(l => l.UserId == userId);
		if (bookId is not null) query = query.Where(l => l.BookId == bookId);
		return query.Any();
	}


	public void Update(Loan loan)
	{
		// In-memory collections update by reference automatically.
		// However, we leave this method empty rather than throwing an exception 
		// so that the Service layer can safely call _repository.Update() 
		// without crashing, simulating a real database save operation.
	}


	public int CountLoans(Guid? userId = null, LoanFilter filter = LoanFilter.Active)
	{
		var query = ApplyFilter(_loans, filter);

		if (userId is not null) query = query.Where(l => l.UserId == userId);
		return query.Count();
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