using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Library;
using LibraryManagementSystem.Domain.Enums.Filters;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class LibraryStatisticsService
{
	private readonly IBookRepository _bookRepository;
	private readonly IAuthorRepository _authorRepository;
	private readonly ITranslatorRepository _translatorRepository;
	private readonly IUserRepository _userRepository;
	private readonly ILoanRepository _loanRepository;


	public LibraryStatisticsService(IBookRepository bookRepository, IAuthorRepository authorRepository,
		ITranslatorRepository translatorRepository,
		IUserRepository userRepository, ILoanRepository loanRepository)
	{
		_bookRepository = bookRepository;
		_authorRepository = authorRepository;
		_translatorRepository = translatorRepository;
		_userRepository = userRepository;
		_loanRepository = loanRepository;
	}


	public ServiceResult<LibraryStatisticsDto> GetLibraryStatistics(ICurrentUserSession session)
	{
		if (session is { IsAdmin: false, IsLibrarian: false })
			return ServiceResult<LibraryStatisticsDto>.Fail(Messages.LibraryStatisticsAccessDenied);

		var stats = new LibraryStatisticsDto
		{
			TotalBooks = _bookRepository.GetAll().Count,
			TotalAuthors = _authorRepository.GetAll(EntityFilter.Active).Count,
			TotalTranslators = _translatorRepository.GetAll(EntityFilter.Active).Count,
			TotalUsers = _userRepository.GetAll(EntityFilter.Active).Count,
			TotalActiveLoans = _loanRepository.CountActiveLoans(),
		};
		return ServiceResult<LibraryStatisticsDto>.Ok(stats, "Computed successfully");
	}
}