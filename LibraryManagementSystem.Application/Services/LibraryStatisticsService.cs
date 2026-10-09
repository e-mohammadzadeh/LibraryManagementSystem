using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Library;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class LibraryStatisticsService
{
	private readonly IBookRepository _bookRepository;
	private readonly IAuthorRepository _authorRepository;
	private readonly ITranslatorRepository _translatorRepository;
	private readonly IUserRepository _userRepository;
	private readonly ILoanManagementService _loanService;


	public LibraryStatisticsService(IBookRepository bookRepository, IAuthorRepository authorRepository,
		ITranslatorRepository translatorRepository,
		IUserRepository userRepository, ILoanManagementService loanService)
	{
		_bookRepository = bookRepository;
		_authorRepository = authorRepository;
		_translatorRepository = translatorRepository;
		_userRepository = userRepository;
		_loanService = loanService;
	}


	public ServiceResult<LibraryStatisticsDto> GetLibraryStatistics(ICurrentUserSession session)
	{
		if (session is { IsAdmin: false, IsLibrarian: false })
			return ServiceResult<LibraryStatisticsDto>.Fail(Messages.LibraryStatisticsAccessDenied);

		var stats = new LibraryStatisticsDto
		{
			TotalBooks = _bookRepository.GetAll(EntityFilter.Active).Count,
			TotalAuthors = _authorRepository.GetAll(EntityFilter.Active).Count,
			TotalTranslators = _translatorRepository.GetAll(EntityFilter.Active).Count,
			TotalUsers = _userRepository.GetAll(EntityFilter.Active).Count,
			TotalActiveLoans = _loanService.CountLoans(null, LoanFilter.Active),
		};
		return ServiceResult<LibraryStatisticsDto>.Ok(stats, "Computed successfully");
	}
}