using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IFineHistoryRepository
{
	void Add(FineHistory history);
	IReadOnlyList<FineHistory> GetByFineId(Guid fineId);
	IReadOnlyList<FineHistory> GetByLoanId(Guid loanId);
	IReadOnlyList<FineHistory> GetByUserId(Guid userId);
	IReadOnlyList<FineHistory> GetAll();
}