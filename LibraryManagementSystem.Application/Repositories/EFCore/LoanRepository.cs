using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class LoanRepository : ILoanRepository
{
	protected readonly LibraryDbContext Context;


	public LoanRepository(LibraryDbContext context) { Context = context; }


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

		var book = loan.Book;
		var user = loan.User;

		Context.Loans.Add(loan);
		Context.SaveChanges();
		Context.Entry(loan).State = EntityState.Detached;

		loan.Book = book;
		loan.User = user;
	}


	public Loan? GetById(Guid id, LoanFilter filter = LoanFilter.Active) =>
		Query(filter).FirstOrDefault(l => l.Id == id);


	public IReadOnlyList<Loan> GetAll(LoanFilter filter = LoanFilter.Active) =>
		[.. Query(filter).OrderByDescending(l => l.CreatedAt)];


	public IReadOnlyList<Loan> GetByUserId(Guid id, LoanFilter filter = LoanFilter.Active) =>
		[.. Query(filter).Where(l => l.UserId == id).OrderByDescending(l => l.CreatedAt)];


	public IReadOnlyList<Loan> GetByBookId(Guid id, LoanFilter filter = LoanFilter.Active) =>
		[.. Query(filter).Where(l => l.BookId == id).OrderByDescending(l => l.CreatedAt)];


	public IReadOnlyList<Loan> GetLoansByBookAndUser(Guid bookId, Guid userId, LoanFilter filter = LoanFilter.Active) =>
		[.. Query(filter).Where(l => l.BookId == bookId && l.UserId == userId).OrderByDescending(l => l.CreatedAt)];


	public void Update(Loan loan)
	{
		ArgumentNullException.ThrowIfNull(loan);

		var tracked = Context.Loans.FirstOrDefault(l => l.Id == loan.Id) ?? throw new LoanNotFoundException(loan.Id);

		tracked.DueDate = loan.DueDate;
		tracked.ReturnDate = loan.ReturnDate;
		tracked.Status = loan.Status;
		tracked.RenewalCount = loan.RenewalCount;
		tracked.IsOverdue = loan.IsOverdue;
		tracked.IsActive = loan.IsActive;
		tracked.Action = loan.Action;
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = loan.UpdatedByUserId;

		Context.SaveChanges();
	}


	private IQueryable<Loan> Query(LoanFilter filter) =>
		ApplyFilter(Context.Loans
			.AsNoTracking()
			.Include(l => l.Book)
			.Include(l => l.User), filter);


	private static IQueryable<Loan> ApplyFilter(IQueryable<Loan> query, LoanFilter filter)
	{
		return filter switch
		{
			LoanFilter.All => query,
			LoanFilter.Active => query.Where(l => l.IsActive && l.ReturnDate == null),
			LoanFilter.Inactive => query.Where(l => !l.IsActive),
			LoanFilter.Overdue => query.Where(l => l.IsOverdue),
			LoanFilter.Returned => query.Where(l => l.ReturnDate != null),
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
	}
}