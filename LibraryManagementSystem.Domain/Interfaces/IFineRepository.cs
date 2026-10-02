using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using static System.Net.WebRequestMethods;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IFineRepository
{
	void Add(Fine fine);
	Fine? FindById(Guid fineId, FineFilter filter);
	IReadOnlyList<Fine> GetAll(FineFilter filter);
	IReadOnlyList<Fine> GetByLoanId(Guid loanId, FineFilter filter);
	IReadOnlyList<Fine> GetByUserId(Guid userId, FineFilter filter);
	decimal GetAmount(Guid userId,FineFilter filter);
	bool HasFines(Guid userId, FineFilter filter);
	IReadOnlyList<Fine> GetHistory();
	IReadOnlyList<Fine> GetHistoryByUserId(Guid userId);
	void Update(Fine fine);
	void Pay(Fine fine);
	void Waive(Fine fine);
}