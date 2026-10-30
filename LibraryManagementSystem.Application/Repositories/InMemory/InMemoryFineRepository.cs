using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryFineRepository : IFineRepository
{
	private readonly List<Fine> _fines;
	public InMemoryFineRepository(ILoanRepository loans) { _fines = CreateSeedData(loans); }


	private static List<Fine> CreateSeedData(ILoanRepository loans)
	{
		var fines = new List<Fine>();
		var returnedLoans = loans.GetAll(LoanFilter.Returned);

		foreach (var loan in returnedLoans)
		{
			if (loan.ReturnDate is null) continue;
			if (loan.ReturnDate <= loan.DueDate) continue;

			var overdueDays = loan.ReturnDate.Value.DayNumber - loan.DueDate.DayNumber;
			if (overdueDays <= 0) continue;

			var amount = FineCalculator(overdueDays);

			var fine = new Fine
			{
				Id = Guid.CreateVersion7(),
				Loan = loan,
				LoanId = loan.Id,
				UserId = loan.UserId,
				OverdueDays = overdueDays,
				Money = Money.Create(amount),
				Status = FineStatus.Unpaid,
				Reason = $"Fine is created for book \"{loan.Book.Title}\" (ID: {loan.Book.Id}).",
			};

			fines.Add(fine);
		}

		return fines;
	}


	public void Add(Fine fine)
	{
		ArgumentNullException.ThrowIfNull(fine);

		fine.Id = Guid.CreateVersion7();
		fine.Money = Money.Create(FineCalculator(fine.OverdueDays));
		fine.Status = FineStatus.Unpaid;
		fine.CreatedAt = DateTime.UtcNow;
		_fines.Add(fine);
	}


	public Fine? GetById(Guid id, FineFilter filter)
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


	public void Update(Fine fine)
	{
		// In-memory implementation:
		// Fine is already tracked by reference.
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