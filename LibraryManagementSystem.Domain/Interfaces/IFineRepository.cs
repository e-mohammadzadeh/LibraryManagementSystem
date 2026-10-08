using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IFineRepository
{
	void Add(Fine fine);
	Fine? FindById(Guid fineId, FineFilter filter);
	IReadOnlyList<Fine> GetAll(FineFilter filter);
	IReadOnlyList<Fine> GetByLoanId(Guid loanId, FineFilter filter);
	IReadOnlyList<Fine> GetByUserId(Guid userId, FineFilter filter);
	decimal GetAmount(Guid userId,FineFilter filter);
	void Update(Fine fine);
	bool HasFines(Guid userId, FineFilter filter);
}