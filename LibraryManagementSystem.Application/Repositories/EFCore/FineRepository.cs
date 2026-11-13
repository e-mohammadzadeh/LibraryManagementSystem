using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class FineRepository : IFineRepository
{
	protected readonly LibraryDbContext Context;


	public FineRepository(LibraryDbContext context) { Context = context; }


	public void Add(Fine fine)
	{
		ArgumentNullException.ThrowIfNull(fine);

		fine.Id = Guid.CreateVersion7();
		fine.Money = Money.Create(FineCalculator(fine.OverdueDays));
		fine.Status = FineStatus.Unpaid;
		fine.PaidAt = null;
		fine.CreatedAt = DateTime.UtcNow;

		var loan = fine.Loan;
		fine.Loan = null!;

		Context.Fines.Add(fine);
		Context.SaveChanges();
		Context.Entry(fine).State = EntityState.Detached;

		fine.Loan = loan;
	}


	public Fine? GetById(Guid id, FineFilter filter = FineFilter.Unpaid) =>
		Query(filter).FirstOrDefault(f => f.Id == id);


	public IReadOnlyList<Fine> GetAll(FineFilter filter = FineFilter.Unpaid) =>
		[.. Query(filter).OrderByDescending(f => f.CreatedAt)];


	public IReadOnlyList<Fine> GetByLoanId(Guid id, FineFilter filter = FineFilter.Unpaid) =>
		[.. Query(filter).Where(f => f.LoanId == id).OrderByDescending(f => f.CreatedAt)];


	public IReadOnlyList<Fine> GetByUserId(Guid id, FineFilter filter = FineFilter.Unpaid) =>
		[.. Query(filter).Where(f => f.UserId == id).OrderByDescending(f => f.CreatedAt)];


	public decimal GetAmount(Guid userId, FineFilter filter = FineFilter.Unpaid)
	{
		var amounts = ApplyFilter(Context.Fines.AsNoTracking(), filter)
			.Where(f => f.UserId == userId)
			.Select(f => f.Money)
			.ToList();

		return amounts.Sum(m => (decimal)m);
	}


	public bool HasFines(Guid userId, FineFilter filter = FineFilter.Unpaid) =>
		ApplyFilter(Context.Fines.AsNoTracking(), filter)
			.Any(f => f.UserId == userId);


	public void Update(Fine fine)
	{
		ArgumentNullException.ThrowIfNull(fine);

		var tracked = Context.Fines.FirstOrDefault(f => f.Id == fine.Id) ?? throw new FineNotFoundException(fine.Id);

		tracked.OverdueDays = fine.OverdueDays;
		tracked.Money = fine.Money;
		tracked.Status = fine.Status;
		tracked.PaidAt = fine.PaidAt;
		tracked.Reason = fine.Reason;
		tracked.Action = fine.Action;
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = fine.UpdatedByUserId;

		Context.SaveChanges();
	}


	private IQueryable<Fine> Query(FineFilter filter) =>
		ApplyFilter(Context.Fines
			.AsNoTracking()
			.Include(f => f.Loan), filter);


	private static IQueryable<Fine> ApplyFilter(IQueryable<Fine> query, FineFilter filter)
	{
		return filter switch
		{
			FineFilter.Paid => query.Where(f => f.Status == FineStatus.Paid),
			FineFilter.Waived => query.Where(f => f.Status == FineStatus.Waived),
			FineFilter.Unpaid => query.Where(f => f.Status == FineStatus.Unpaid),
			FineFilter.All => query,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
	}


	private static decimal FineCalculator(int overdueDays)
	{
		if (overdueDays <= 0) return 0m;

		var flatTotal = Math.Min(overdueDays, ValidationConstants.FixedRateDays) * ValidationConstants.InitialDailyRate;

		if (overdueDays <= ValidationConstants.FixedRateDays)
			return Math.Min(flatTotal, ValidationConstants.MaxUnpaidFineThreshold);

		var geometricDays = overdueDays - ValidationConstants.FixedRateDays;
		var geometricTotal = ValidationConstants.InitialDailyRate *
		                     ((decimal)Math.Pow((double)ValidationConstants.GeometricRatio, geometricDays) - 1m) /
		                     (ValidationConstants.GeometricRatio - 1m);

		var total = flatTotal + geometricTotal;
		return Math.Min(total, ValidationConstants.MaxUnpaidFineThreshold);
	}
}