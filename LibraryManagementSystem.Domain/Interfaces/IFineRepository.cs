using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IFineRepository
{
	void Add(Fine fine);
	Fine? FindById(int fineId);
	IReadOnlyList<Fine> GetAll();
	IReadOnlyList<Fine> GetAllUnpaid();
	IReadOnlyList<Fine> GetByLoanId(int loanId);
	IReadOnlyList<Fine> GetByUserId(int userId);
	IReadOnlyList<Fine> GetUnpaidByUserId(int userId);
	bool HasUnpaidFines(int userId);
	decimal GetTotalUnpaidAmount(int userId);
	void Update(Fine fine);
}