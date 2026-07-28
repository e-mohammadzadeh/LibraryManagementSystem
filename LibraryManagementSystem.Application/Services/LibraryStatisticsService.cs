using LibraryManagementSystem.Application.DTOs.Library;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class LibraryStatisticsService
{
	private readonly IBookRepository _bookRepository;
	private readonly IAuthorRepository _authorRepository;
	private readonly ITranslatorRepository _translatorRepository;
	private readonly IUserRepository _userRepository;
	private readonly ILoanRepository _loanRepository;


	public LibraryStatisticsService(IBookRepository bookRepository, IAuthorRepository authorRepository, ITranslatorRepository translatorRepository,
		IUserRepository userRepository, ILoanRepository loanRepository)
	{
		_bookRepository = bookRepository;
		_authorRepository = authorRepository;
		_translatorRepository = translatorRepository;
		_userRepository = userRepository;
		_loanRepository = loanRepository;
	}


	public LibraryStatisticsDto GetLibraryStatistics()
	{
		return new LibraryStatisticsDto
		{
			TotalBooks = _bookRepository.GetAll().Count,
			TotalAuthors = _authorRepository.GetAll().Count,
			TotalTranslators = _translatorRepository.GetAll().Count,
			TotalUsers = _userRepository.GetAll().Count,
			TotalActiveLoans = _loanRepository.CountActiveLoans(),
		};
	}
}