using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IFineRepository
{
	void Add(Fine fine);
	Fine? FindById(Guid fineId);
	IReadOnlyList<Fine> GetAllUnpaid();
	IReadOnlyList<Fine> GetByLoanId(Guid loanId);
	IReadOnlyList<Fine> GetByUserId(Guid userId);
	IReadOnlyList<Fine> GetUnpaidByUserId(Guid userId);
	bool HasUnpaidFines(Guid userId);
	decimal GetTotalUnpaidAmount(Guid userId);
	IReadOnlyList<Fine> GetHistory();
	IReadOnlyList<Fine> GetHistoryByUserId(Guid userId);
	void Update(Fine fine);
	void Pay(Fine fine);
	void Waive(Fine fine);
}