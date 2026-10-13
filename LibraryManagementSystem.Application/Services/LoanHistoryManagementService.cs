using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.DTOs.Loans;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class LoanHistoryManagementService : ILoanHistoryManagementService
{
	private readonly ILoanHistoryRepository _loanHistoryRepository;
	private readonly IUserRepository _userRepository;
	private readonly IBookRepository _bookRepository;


	public LoanHistoryManagementService(ILoanHistoryRepository loanHistoryRepository, IUserRepository userRepository,
		IBookRepository bookRepository)
	{
		_loanHistoryRepository = loanHistoryRepository;
		_userRepository = userRepository;
		_bookRepository = bookRepository;
	}


	public void Record(LoanDto loan, LoanHistoryAction action, string? description = null)
	{
		var history = new LoanHistory
		{
			Id = Guid.CreateVersion7(),
			LoanId = loan.Id,
			UserId = loan.UserId,
			BookId = loan.BookId,
			Action = action,
			OccurredAt = DateTime.UtcNow,
			Description = description
		};

		_loanHistoryRepository.Add(history);
	}


	public IReadOnlyList<LoanHistoryDto> GetAll()
	{
		var histories = _loanHistoryRepository.GetAll();
		return MapWithLookups(histories);
	}


	public IReadOnlyList<LoanHistoryDto> GetByBookId(Guid bookId)
	{
		var histories = _loanHistoryRepository.GetByBookId(bookId);
		return MapWithLookups(histories);
	}


	public IReadOnlyList<LoanHistoryDto> GetByLoanId(Guid loanId)
	{
		var histories = _loanHistoryRepository.GetByLoanId(loanId);
		return MapWithLookups(histories);
	}


	public IReadOnlyList<LoanHistoryDto> GetByUserId(Guid userId)
	{
		var histories = _loanHistoryRepository.GetByUserId(userId);
		return MapWithLookups(histories);
	}


	private IReadOnlyList<LoanHistoryDto> MapWithLookups(IEnumerable<LoanHistory> histories)
	{
		var userNames = _userRepository.GetAll(EntityFilter.All)
			.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}");

		var bookTitles = _bookRepository.GetAll(EntityFilter.All)
			.ToDictionary(b => b.Id, b => b.Title);

		return
		[
			.. histories.Select(h =>
				h.ToDto(
					userNames.GetValueOrDefault(h.UserId, "Unknown User"),
					bookTitles.GetValueOrDefault(h.BookId, "Unknown Book")))
		];
	}
}