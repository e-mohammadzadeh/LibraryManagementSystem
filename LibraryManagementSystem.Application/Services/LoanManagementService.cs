using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Loans;
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


	public ServiceResult<LoanDto> BorrowBook(int userId, int bookId)
	{
		var user = _userRepository.FindById(userId);
		if (user is null) return ServiceResult<LoanDto>.Fail(ValidationMessages.NotUserMatched);

		if (!user.IsActive) return ServiceResult<LoanDto>.Fail(ValidationMessages.MembershipExpired);

		var book = _bookRepository.FindById(bookId);
		if (book is null) return ServiceResult<LoanDto>.Fail(ValidationMessages.NotBookMatched);

		if (book.AvailableCopies <= 0) return ServiceResult<LoanDto>.Fail(ValidationMessages.NotEnoughCopiesAvailable);

		if (_loanRepository.CountActiveLoansByUser(userId) >= ValidationConstants.MaxActiveLoansPerUser)
			return ServiceResult<LoanDto>.Fail(ValidationMessages.MaximumLoansReached);

		if (_loanRepository.HasActiveLoan(userId, bookId))
			return ServiceResult<LoanDto>.Fail(ValidationMessages.BookAlreadyBorrowed);

		var loan = new Loan(book, user);
		book.BorrowCopy();
		_loanRepository.Add(loan);
		return ServiceResult<LoanDto>.Ok(MapToDto(loan), ValidationMessages.BorrowedSuccessfully);
	}


	public ServiceResult<LoanDto> ReturnBook(int loanId)
	{
		var loan = _loanRepository.GetActiveLoanById(loanId);
		if (loan is null) return ServiceResult<LoanDto>.Fail(ValidationMessages.ActiveLoanNotFound);

		loan.MarkAsReturned();
		loan.Book.ReturnCopy();
		return ServiceResult<LoanDto>.Ok(MapToDto(loan), ValidationMessages.ReturnedSuccessfully);
	}


	public IReadOnlyList<Loan> GetActiveLoansByUser(int userId) { return _loanRepository.GetActiveLoansByUser(userId); }


	public IReadOnlyList<Loan> GetActiveLoansByBook(int bookId) { return _loanRepository.GetActiveLoansByBook(bookId); }


	public ServiceResult<Loan> RenewLoan(int loanId)
	{
		var loan = _loanRepository.GetActiveLoanById(loanId);
		if (loan is null) return ServiceResult<Loan>.Fail(ValidationMessages.ActiveLoanNotFound);

		if (!loan.CanRenew(out var errorMessage)) return ServiceResult<Loan>.Fail(errorMessage);

		loan.Renew();
		return ServiceResult<Loan>.Ok(loan, ValidationMessages.RenewedSuccessfully);
	}


	public IReadOnlyList<Loan> GetOverdueLoans() { return _loanRepository.GetOverdueLoans(); }


	public IReadOnlyList<Loan> GetLoansByUser(int userId) { return _loanRepository.GetAllByUser(userId); }


	public IReadOnlyList<Loan> SearchLoans<T>(T? searchTerm, Func<Loan, T?> selector, Func<T, T, bool> comparer)
		where T : class
	{
		// TODO: When EF is added, move search filtering to ILoanRepository.Search<T>() to allow SQL-level filtering instead of in-memory LINQ.

		if (searchTerm is null) return [];

		return _loanRepository.GetAll().Where(l =>
		{
			var value = selector(l);
			return value != null && comparer(searchTerm, value);
		}).ToList().AsReadOnly();
	}


	public IReadOnlyList<Loan> SearchLoans<T>(T? searchTerm, Func<Loan, T?> selector, Func<T, T, bool> comparer)
		where T : struct
	{
		if (!searchTerm.HasValue) return [];

		return _loanRepository.GetAll().Where(l =>
		{
			var value = selector(l);
			return value.HasValue && comparer(searchTerm.Value, value.Value);
		}).ToList().AsReadOnly();
	}


	public IReadOnlyList<Loan> SearchActiveLoans<T>(T? searchTerm, Func<Loan, T?> selector, Func<T, T, bool> comparer)
		where T : class
	{
		if (searchTerm is null) return [];

		return _loanRepository.GetActiveLoans().Where(l =>
		{
			var value = selector(l);
			return value != null && comparer(searchTerm, value);
		}).ToList().AsReadOnly();
	}


	public IReadOnlyList<Loan> SearchActiveLoans<T>(T? searchTerm, Func<Loan, T?> selector, Func<T, T, bool> comparer)
		where T : struct
	{
		if (!searchTerm.HasValue) return [];

		return _loanRepository.GetActiveLoans().Where(l =>
		{
			var value = selector(l);
			return value.HasValue && comparer(searchTerm.Value, value.Value);
		}).ToList().AsReadOnly();
	}


	public IReadOnlyList<LoanDto> GetAllLoans()
	{
		return _loanRepository.GetAll().Select(MapToDto).ToList().AsReadOnly();
	}



	public IReadOnlyList<Loan> GetLoanByBook(int bookId) { return _loanRepository.GetLoansByBook(bookId); }


	public IReadOnlyList<LoanDto> GetAllActiveLoans()
	{
		return _loanRepository.GetActiveLoans().Select(MapToDto).ToList().AsReadOnly();
	}


	private static LoanDto MapToDto(Loan loan)
	{
		return new LoanDto
		{
			LoanId = loan.LoanId,
			BookName = loan.Book.BookName,
			BorrowDate = loan.BorrowDate,
			DueDate = loan.DueDate,
			ReturnDate = loan.ReturnDate,
			Status = loan.Status.ToString(),
			RenewalCount = loan.RenewalCount,
			IsOverdue = loan.IsOverdue,
			CreatedAt = loan.CreatedAt,
			UpdatedAt = loan.UpdatedAt
		};
	}
}