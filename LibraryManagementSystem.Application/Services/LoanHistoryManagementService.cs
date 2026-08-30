using LibraryManagementSystem.Application.DTOs.Loans;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class LoanHistoryManagementService : ILoanHistoryManagementService
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
		return [.. _loanHistoryRepository.GetAll().Select(history => history.ToDto())];
	}


	public IReadOnlyList<LoanHistoryDto> GetByBookId(int bookId)
	{
		return [.. _loanHistoryRepository.GetByBookId(bookId).Select(history => history.ToDto())];
	}


	public IReadOnlyList<LoanHistoryDto> GetByLoanId(int loanId)
	{
		return [.. _loanHistoryRepository.GetByLoanId(loanId).Select(history => history.ToDto())];
	}


	public IReadOnlyList<LoanHistoryDto> GetByUserId(int userId)
	{
		return [.. _loanHistoryRepository.GetByUserId(userId).Select(history => history.ToDto())];
	}
}