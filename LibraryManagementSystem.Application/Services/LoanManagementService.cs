using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class LoanManagementService
{
	private readonly ILoanRepository _loanRepository;
	private readonly IUserRepository _userRepository;
	private readonly IBookRepository _bookRepository;


	public LoanManagementService(ILoanRepository loanRepository, IUserRepository userRepository,
		IBookRepository bookRepository)
	{
		_loanRepository = loanRepository;
		_userRepository = userRepository;
		_bookRepository = bookRepository;
	}


	public ServiceResult<Loan> BorrowBook(int userId, int bookId)
	{
		var user = _userRepository.FindById(userId);
		if (user is null)
			return ServiceResult<Loan>.Fail(ValidationMessages.NotUserMatched);

		if (!user.IsActive)
			return ServiceResult<Loan>.Fail(ValidationMessages.MembershipExpired);

		var book = _bookRepository.FindById(bookId);
		if (book is null)
			return ServiceResult<Loan>.Fail(ValidationMessages.NotBookMatched);

		if (book.AvailableCopies <= 0)
			return ServiceResult<Loan>.Fail(ValidationMessages.NotEnoughCopiesAvailable);

		if (_loanRepository.CountActiveLoansByUser(userId) >= ValidationConstants.MaxActiveLoansPerUser)
			return ServiceResult<Loan>.Fail(ValidationMessages.MaximumLoansReached);

		if (_loanRepository.HasActiveLoan(userId, bookId))
			return ServiceResult<Loan>.Fail(ValidationMessages.BookAlreadyBorrowed);

		var loan = new Loan(book, user);
		book.BorrowCopy();
		_loanRepository.Add(loan);
		return ServiceResult<Loan>.Ok(loan, ValidationMessages.BorrowedSuccessfully);
	}


	public ServiceResult<Loan> ReturnBook(int loanId)
	{
		var loan = _loanRepository.GetActiveLoanById(loanId);
		if (loan is null)
			return ServiceResult<Loan>.Fail(ValidationMessages.ActiveLoanNotFound);

		loan.MarkAsReturned();
		loan.Book.ReturnCopy();
		return ServiceResult<Loan>.Ok(loan, ValidationMessages.ReturnedSuccessfully);
	}


	public IReadOnlyList<Loan> GetActiveLoansByUser(int userId)
	{
		return _loanRepository.GetActiveLoansByUser(userId);
	}


	public ServiceResult<Loan> RenewLoan(int loanId)
	{
		var loan = _loanRepository.GetActiveLoanById(loanId);
		if (loan is null)
			return ServiceResult<Loan>.Fail(ValidationMessages.ActiveLoanNotFound);

		if (!loan.CanRenew(out var errorMessage))
			return ServiceResult<Loan>.Fail(errorMessage);

		loan.Renew();
		return ServiceResult<Loan>.Ok(loan, ValidationMessages.RenewedSuccessfully);
	}


	public IReadOnlyList<Loan> GetOverdueLoans()
	{
		return _loanRepository.GetOverdueLoans();
	}


	public IReadOnlyList<Loan> GetLoansByUser(int userId)
	{
		return _loanRepository.GetAllByUser(userId);
	}


	public IReadOnlyList<Loan> SearchLoans(string searchTerm)
	{
		if (string.IsNullOrWhiteSpace(searchTerm)) return _loanRepository.GetAll();

		return _loanRepository.GetAll()
			.Where(l => l.Book.BookName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
			            l.User.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
			.ToList()
			.AsReadOnly();
	}


	public IReadOnlyList<Loan> GetAllLoans()
	{
		return _loanRepository.GetAll();
	}



	public IReadOnlyList<Loan> GetLoanByBook(int bookId)
	{
		return _loanRepository.GetLoansByBook(bookId);
	}

	public IReadOnlyList<Loan> GetAllActiveLoans() 
	{
		return _loanRepository.GetActiveLoans();
	}
}