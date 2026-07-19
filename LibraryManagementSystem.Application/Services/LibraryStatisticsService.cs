using LibraryManagementSystem.Application.DTOs.Library;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class LibraryStatisticsService
{
	private readonly IBookRepository _bookRepository;
	private readonly IAuthorRepository _authorRepository;
	private readonly IUserRepository _userRepository;
	private readonly ILoanRepository _loanRepository;


	public LibraryStatisticsService(IBookRepository bookRepository, IAuthorRepository authorRepository,
		IUserRepository userRepository, ILoanRepository loanRepository)
	{
		_bookRepository = bookRepository;
		_authorRepository = authorRepository;
		_userRepository = userRepository;
		_loanRepository = loanRepository;
	}


	public LibraryStatisticsDto GetLibraryStatistics()
	{
		return new LibraryStatisticsDto
		{
			TotalBooks = _bookRepository.Count(),
			TotalAuthors = _authorRepository.Count(),
			TotalUsers = _userRepository.Count(),
			TotalActiveLoans = _loanRepository.CountActiveLoans(),
		};
	}
}