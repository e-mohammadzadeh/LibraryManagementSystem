using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IFineRepository
{
	void Add(Fine fine);
	Fine? FindById(int findId);
	IReadOnlyList<Fine> GetAll();
	IReadOnlyList<Fine> GetAllUnpaid();
	IReadOnlyList<Fine> GetByUserId(int userId);
	IReadOnlyList<Fine> GetUnpaidByUserId(int userId);
	IReadOnlyList<Fine> GetUnpaidByLoanId(int loanId);
	bool HasUnpaidFines(int userId);
	decimal GetTotalUnpaidAmount(int userId);
	void Remove(Fine fine);
	void Update(Fine fine);
}