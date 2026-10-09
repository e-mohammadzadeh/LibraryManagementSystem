using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.DTOs.Fine;
using LibraryManagementSystem.Infrastructure.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class FineHistoryManagementService: IFineHistoryManagementService
{
	private readonly IFineHistoryRepository _fineHistoryRepository;


	public FineHistoryManagementService(IFineHistoryRepository fineHistoryRepository)
	{
		_fineHistoryRepository = fineHistoryRepository;
	}


	public void Record(FineDto fine, FineHistoryAction action, string? description = null)
	{
		var history = new FineHistory
		{
			Id = Guid.CreateVersion7(),
			FineId = fine.FineId!.Value,
			LoanId = fine.LoanId!.Value,
			UserId = fine.UserId!.Value,
			OverdueDays = fine.OverdueDays!.Value,
			Money = fine.Money!,
			Status = fine.Status!.Value,
			Action = action,
			OccurredAt = DateTime.UtcNow,
			Description = description
		};

		_fineHistoryRepository.Add(history);
	}


	public IReadOnlyList<FineHistoryDto> GetAll()
	{
		return [.. _fineHistoryRepository.GetAll().Select(history => history.ToDto())];
	}


	public IReadOnlyList<FineHistoryDto> GetByFineId(Guid fineId)
	{
		return [.. _fineHistoryRepository.GetByFineId(fineId).Select(history => history.ToDto())];
	}


	public IReadOnlyList<FineHistoryDto> GetByLoanId(Guid loanId)
	{
		return [.. _fineHistoryRepository.GetByLoanId(loanId).Select(history => history.ToDto())];
	}


	public IReadOnlyList<FineHistoryDto> GetByUserId(Guid userId)
	{
		return [.. _fineHistoryRepository.GetByUserId(userId).Select(history => history.ToDto())];
	}
}