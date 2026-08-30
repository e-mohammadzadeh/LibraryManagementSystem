using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class LoanHistoryManagementService
{
	private readonly ILoanHistoryRepository _loanHistoryRepository;


	public LoanHistoryManagementService(ILoanHistoryRepository loanHistoryRepository)
	{
		_loanHistoryRepository = loanHistoryRepository;
	}


	public void Record(Loan loan, LoanHistoryAction action, string? description = null)
	{
		var history = new LoanHistory(loan, action, description);
		_loanHistoryRepository.Add(history);
	}


	public IReadOnlyList<LoanHistoryDto> GetAll()
	{

	}
}