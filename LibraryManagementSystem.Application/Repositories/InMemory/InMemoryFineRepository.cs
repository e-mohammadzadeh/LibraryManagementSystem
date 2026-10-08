using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryFineRepository : IFineRepository
{
	private readonly List<Fine> _fines = [];


	public void Add(Fine fine)
	{
		ArgumentNullException.ThrowIfNull(fine);

		fine.Id = Guid.CreateVersion7();
		fine.Money = Money.Create(FineCalculator(fine.OverdueDays));
		fine.Status = FineStatus.Unpaid;
		fine.CreatedAt = DateTime.UtcNow;
		_fines.Add(fine);
	}


	public Fine? FindById(Guid id, FineFilter filter)
	{
		return ApplyFilter(_fines, filter).FirstOrDefault(f => f.Id == id);
	}


	public IReadOnlyList<Fine> GetAll(FineFilter filter = FineFilter.Unpaid)
	{
		return [.. ApplyFilter(_fines, filter)];
	}


	public IReadOnlyList<Fine> GetByLoanId(Guid loanId, FineFilter filter = FineFilter.Unpaid)
	{
		return [.. ApplyFilter(_fines, filter).Where(f => f.LoanId == loanId)];
	}


	public IReadOnlyList<Fine> GetByUserId(Guid userId, FineFilter filter = FineFilter.Unpaid)
	{
		return [.. ApplyFilter(_fines, filter).Where(f => f.UserId == userId)];
	}


	public decimal GetAmount(Guid userId, FineFilter filter = FineFilter.Unpaid)
	{
		return ApplyFilter(_fines, filter).Where(f => f.UserId == userId).Sum(f => f.Money);
	}


	public bool HasFines(Guid userId, FineFilter filter = FineFilter.Unpaid)
	{
		return ApplyFilter(_fines, filter).Any(f => f.UserId == userId);
	}


	public IReadOnlyList<Fine> GetHistory(FineFilter filter = FineFilter.Unpaid)
	{
		return [.. ApplyFilter(_fines, filter)];
	}


	public IReadOnlyList<Fine> GetHistoryByUserId(Guid userId, FineFilter filter = FineFilter.Unpaid)
	{
		return [.. ApplyFilter(_fines, filter).Where(f => f.UserId == userId)];
	}


	public void Update(Fine fine)
	{
		// In-memory implementation:
		// Fine is already tracked by reference.
	}


	public void Pay(Fine fine)
	{
		if (fine.Status == FineStatus.Paid) throw new InvalidOperationException("Fine is already paid.");
		if (fine.Status == FineStatus.Waived) throw new InvalidOperationException("Fine has been waived.");
		fine.Status = FineStatus.Paid;
		fine.PaidAt = DateOnly.FromDateTime(DateTime.Today);
		fine.UpdatedAt = DateTime.Now;
	}


	public void Waive(Fine fine)
	{
		if (fine.Status == FineStatus.Paid) throw new InvalidOperationException("Cannot waive an already paid fine.");
		fine.Status = FineStatus.Waived;
		fine.UpdatedAt = DateTime.Now;
	}


	// ---------- Private helper ----------
	private static IEnumerable<Fine> ApplyFilter(IEnumerable<Fine> source, FineFilter filter)
	{
		return filter switch
		{
			FineFilter.Paid => source.Where(f => f.Status == FineStatus.Paid),
			FineFilter.Unpaid => source.Where(f => f.Status == FineStatus.Unpaid),
			FineFilter.Waived => source.Where(f => f.Status == FineStatus.Waived),
			FineFilter.All => source,
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