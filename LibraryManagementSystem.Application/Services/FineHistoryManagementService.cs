using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.DTOs.Fine;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class FineHistoryManagementService : IFineHistoryManagementService
{
	private readonly IFineHistoryRepository _fineHistoryRepository;
	private readonly IUserRepository _userRepository;


	public FineHistoryManagementService(IFineHistoryRepository fineHistoryRepository, IUserRepository userRepository)
	{
		_fineHistoryRepository = fineHistoryRepository;
		_userRepository = userRepository;
	}


	public void Record(FineDto fine, FineHistoryAction action, string? description = null)
	{
		var history = new FineHistory
		{
			Id = Guid.CreateVersion7(),
			FineId = fine.Id,
			LoanId = fine.LoanId,
			UserId = fine.UserId,
			OverdueDays = fine.OverdueDays,
			Money = fine.Money,
			Status = fine.Status,
			Action = action,
			OccurredAt = DateTime.UtcNow,
			Description = description
		};

		_fineHistoryRepository.Add(history);
	}


	public IReadOnlyList<FineHistoryDto> GetAll() => MapWithLookups(_fineHistoryRepository.GetAll());


	public IReadOnlyList<FineHistoryDto> GetByFineId(Guid fineId) =>
		MapWithLookups(_fineHistoryRepository.GetByFineId(fineId));


	public IReadOnlyList<FineHistoryDto> GetByLoanId(Guid loanId) =>
		MapWithLookups(_fineHistoryRepository.GetByLoanId(loanId));


	public IReadOnlyList<FineHistoryDto> GetByUserId(Guid userId) =>
		MapWithLookups(_fineHistoryRepository.GetByUserId(userId));


	private IReadOnlyList<FineHistoryDto> MapWithLookups(IEnumerable<FineHistory> histories)
	{
		var list = histories.ToList();

		var userNames = _userRepository.GetAll(EntityFilter.All)
			.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}");

		return
		[
			.. list.Select(h => h.ToDto(
				userNames.GetValueOrDefault(h.UserId, "Unknown User")))
		];
	}
}