using LibraryManagementSystem.Application.DTOs.Fine;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class FineHistoryManagementService
{
	private readonly IFineHistoryRepository _fineHistoryRepository;


	public FineHistoryManagementService(IFineHistoryRepository fineHistoryRepository)
	{
		_fineHistoryRepository = fineHistoryRepository;
	}


	public void Record(Fine fine, FineHistoryAction action, string? description = null)
	{
		var history = new FineHistory(fine, action, description);
		_fineHistoryRepository.Add(history);
	}


	public IReadOnlyList<FineHistoryDto> GetAll()
	{
		return [.. _fineHistoryRepository.GetAll().Select(history => history.ToDto())];
	}


	public IReadOnlyList<FineHistoryDto> GetByFineId(int fineId)
	{
		return [.. _fineHistoryRepository.GetByFineId(fineId).Select(history => history.ToDto())];
	}


	public IReadOnlyList<FineHistoryDto> GetByLoanId(int loanId)
	{
		return [.. _fineHistoryRepository.GetByLoanId(loanId).Select(history => history.ToDto())];
	}


	public IReadOnlyList<FineHistoryDto> GetByUserId(int userId)
	{
		return [.. _fineHistoryRepository.GetByUserId(userId).Select(history => history.ToDto())];
	}
}