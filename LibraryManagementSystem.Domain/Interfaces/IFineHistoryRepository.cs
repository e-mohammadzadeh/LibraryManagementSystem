using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IFineHistoryRepository
{
	void Add(FineHistory history);
	IReadOnlyList<FineHistory> GetByFineId(int fineId);
	IReadOnlyList<FineHistory> GetByLoanId(int loanId);
	IReadOnlyList<FineHistory> GetByUserId(int userId);
	IReadOnlyList<FineHistory> GetAll();
}