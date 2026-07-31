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
	private readonly IFineRepository _fineRepository;
	private readonly FineManagementService _fineService;


	public LoanManagementService(ILoanRepository loanRepository, IUserRepository userRepository,
		IBookRepository bookRepository, IFineRepository fineRepository, FineManagementService fineManagementService)
	{
		_loanRepository = loanRepository;
		_userRepository = userRepository;
		_bookRepository = bookRepository;
		_fineRepository = fineRepository;
		_fineService = fineManagementService;
	}


	public ServiceResult<LoanDto> BorrowBook(int userId, int bookId)
	{
		var user = _userRepository.FindById(userId);
		if (user is null) return ServiceResult<LoanDto>.Fail(ValidationMessages.NotUserMatched);

		if (!user.IsActive) return ServiceResult<LoanDto>.Fail(ValidationMessages.MembershipExpired);

		if (user.ShouldRemove) return ServiceResult<LoanDto>.Fail(ValidationMessages.Flagged4Removal);

		if (_fineRepository.HasUnpaidFines(userId))
			return ServiceResult<LoanDto>.Fail(ValidationMessages.BorrowFailed4Fine);

		if (_loanRepository.CountActiveLoansByUser(userId) >= ValidationConstants.MaxActiveLoansPerUser)
			return ServiceResult<LoanDto>.Fail(ValidationMessages.MaximumLoansReached);

		var book = _bookRepository.FindById(bookId);
		if (book is null) return ServiceResult<LoanDto>.Fail(ValidationMessages.NotBookMatched);

		if (book.AvailableCopies <= 0) return ServiceResult<LoanDto>.Fail(ValidationMessages.NotEnoughCopiesAvailable);

		if (_loanRepository.HasActiveLoan(userId, bookId))
			return ServiceResult<LoanDto>.Fail(ValidationMessages.BookAlreadyBorrowed);

		var loan = new Loan(book, user, DateOnly.FromDateTime(DateTime.Today));
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
		_loanRepository.Update(loan);

		var fineResult = _fineService.CreateFineForLoan(loanId);
		if (!fineResult.Success && fineResult.Message != ValidationMessages.NoFine)
			return ServiceResult<LoanDto>.Warning(MapToDto(loan),
				$"{ValidationMessages.ReturnedSuccessfully} | Note: {fineResult.Message}");

		return ServiceResult<LoanDto>.Ok(MapToDto(loan), ValidationMessages.ReturnedSuccessfully);
	}


	public IReadOnlyList<LoanDto> GetActiveLoansByUser(int userId)
	{
		return _loanRepository.GetActiveLoansByUser(userId).Select(MapToDto).ToList().AsReadOnly();
	}


	public ServiceResult<LoanDto> RenewLoan(int loanId)
	{
		var loan = _loanRepository.GetActiveLoanById(loanId);
		if (loan is null) return ServiceResult<LoanDto>.Fail(ValidationMessages.ActiveLoanNotFound);

		if (!loan.CanRenew(out var errorMessage)) return ServiceResult<LoanDto>.Fail(errorMessage);

		if (_fineService.HasUnpaidFines(loan.UserId))
			return ServiceResult<LoanDto>.Fail(ValidationMessages.UserHasUnpaidFines);

		loan.Renew();
		_loanRepository.Update(loan);
		return ServiceResult<LoanDto>.Ok(MapToDto(loan), ValidationMessages.RenewedSuccessfully);
	}


	public IReadOnlyList<LoanDto> GetOverdueLoans()
	{
		return _loanRepository.GetOverdueLoans().Select(MapToDto).ToList().AsReadOnly();
	}


	public IReadOnlyList<LoanDto> GetLoansByUser(int userId)
	{
		return _loanRepository.GetAllByUser(userId).Select(MapToDto).ToList().AsReadOnly();
	}


	public IReadOnlyList<LoanDto> SearchLoans<T>(T? searchTerm, Func<Loan, T?> selector, Func<T, T, bool> comparer)
		where T : class
	{
		// TODO	(EF)	When EF is added, move search filtering to ILoanRepository.Search<T>() to allow SQL-level filtering instead of in-memory LINQ.

		if (searchTerm is null) return [];

		return _loanRepository.GetAll().Where(l =>
		{
			var value = selector(l);
			return value != null && comparer(searchTerm, value);
		}).Select(MapToDto).ToList().AsReadOnly();
	}


	public IReadOnlyList<LoanDto> SearchLoans<T>(T? searchTerm, Func<Loan, T?> selector, Func<T, T, bool> comparer)
		where T : struct
	{
		if (!searchTerm.HasValue) return [];

		return _loanRepository.GetAll().Where(l =>
		{
			var value = selector(l);
			return value.HasValue && comparer(searchTerm.Value, value.Value);
		}).Select(MapToDto).ToList().AsReadOnly();
	}


	public IReadOnlyList<LoanDto> SearchActiveLoans<T>(T? searchTerm, Func<Loan, T?> selector,
		Func<T, T, bool> comparer)
		where T : class
	{
		if (searchTerm is null) return [];

		return _loanRepository.GetActiveLoans().Where(l =>
		{
			var value = selector(l);
			return value != null && comparer(searchTerm, value);
		}).Select(MapToDto).ToList().AsReadOnly();
	}


	public IReadOnlyList<LoanDto> SearchActiveLoans<T>(T? searchTerm, Func<Loan, T?> selector,
		Func<T, T, bool> comparer)
		where T : struct
	{
		if (!searchTerm.HasValue) return [];

		return _loanRepository.GetActiveLoans().Where(l =>
		{
			var value = selector(l);
			return value.HasValue && comparer(searchTerm.Value, value.Value);
		}).Select(MapToDto).ToList().AsReadOnly();
	}


	public IReadOnlyList<LoanDto> GetAllLoans()
	{
		return _loanRepository.GetAll().Select(MapToDto).ToList().AsReadOnly();
	}


	public IReadOnlyList<LoanDto> GetLoanByBook(int bookId)
	{
		return _loanRepository.GetLoansByBook(bookId).Select(MapToDto).ToList().AsReadOnly();
	}


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
			BookId = loan.BookId,
			UserName = $"{loan.User.FirstName} {loan.User.LastName}",
			UserId = loan.UserId,
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