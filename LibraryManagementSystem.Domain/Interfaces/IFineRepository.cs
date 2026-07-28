using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IFineRepository
{
	void Add(Fine fine);
	Fine? FindById(int findId);
	IReadOnlyList<Fine> GetAll();
	IReadOnlyList<Fine> GetByUserId(int userId);
	IReadOnlyList<Fine> GetUnpaidByUserId(int userId);
	IReadOnlyList<Fine> GetUnpaidByLoanId(int loanId);
	bool HasUnpaidFines(int userId);
	decimal GetTotalUnpaidAmound(int userId);
	void Remove(Fine fine);
	void Update(Fine fine);
	int Count();
}