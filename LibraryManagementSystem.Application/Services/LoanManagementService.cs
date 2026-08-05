using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Loans;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class LoanManagementService
{
	private readonly ILoanRepository _loanRepository;
	private readonly IUserRepository _userRepository;
	private readonly IBookRepository _bookRepository;
	private readonly IFineRepository _fineRepository;
	private readonly IFineManagementService _fineService;


	public LoanManagementService(ILoanRepository loanRepository, IUserRepository userRepository,
		IBookRepository bookRepository, IFineRepository fineRepository, IFineManagementService fineManagementService)
	{
		_loanRepository = loanRepository;
		_userRepository = userRepository;
		_bookRepository = bookRepository;
		_fineRepository = fineRepository;
		_fineService = fineManagementService;
	}


	public ServiceResult<LoanDto> BorrowBook(int userId, int bookId, ICurrentUserSession session)
	{
		if (session.IsSelfServiceMember && session.UserId != userId)
			return ServiceResult<LoanDto>.Fail(ValidationMessages.BorrowBook4Yourself);

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
		return ServiceResult<LoanDto>.Ok(loan.ToDto(), ValidationMessages.BorrowedSuccessfully);
	}


	public ServiceResult<LoanDto> ReturnBook(int loanId, ICurrentUserSession session)
	{
		var loan = _loanRepository.GetActiveLoanById(loanId);
		if (loan is null) return ServiceResult<LoanDto>.Fail(ValidationMessages.ActiveLoanNotFound);

		if (session.IsSelfServiceMember && session.UserId != loan.UserId)
			return ServiceResult<LoanDto>.Fail(ValidationMessages.ReturnOwnLoans);

		loan.MarkAsReturned();
		loan.Book.ReturnCopy();
		_loanRepository.Update(loan);

		var fineResult = _fineService.CreateFineForLoan(loanId);
		if (!fineResult.Success && fineResult.Message != ValidationMessages.NoFine)
			return ServiceResult<LoanDto>.Warning(loan.ToDto(),
				$"{ValidationMessages.ReturnedSuccessfully} - Note: {fineResult.Message}");

		return ServiceResult<LoanDto>.Ok(loan.ToDto(), ValidationMessages.ReturnedSuccessfully);
	}


	public ServiceResult<IReadOnlyList<LoanDto>> GetActiveLoansByUser(int userId, ICurrentUserSession session)
	{
		if (session.IsSelfServiceMember && session.UserId != userId)
			return ServiceResult<IReadOnlyList<LoanDto>>.Fail(ValidationMessages.ViewOwnLoans);

		var loans = _loanRepository.GetActiveLoansByUser(userId).Select(loan => loan.ToDto()).ToList().AsReadOnly();
		return ServiceResult<IReadOnlyList<LoanDto>>.Ok(loans, "Loans retrieved successfully.");
	}


	public ServiceResult<LoanDto> RenewLoan(int loanId, ICurrentUserSession session)
	{
		var loan = _loanRepository.GetActiveLoanById(loanId);
		if (loan is null) return ServiceResult<LoanDto>.Fail(ValidationMessages.ActiveLoanNotFound);

		if (session.IsSelfServiceMember && session.UserId != loan.UserId)
			return ServiceResult<LoanDto>.Fail(ValidationMessages.RenewOwnLoans);

		if (!loan.CanRenew(out var errorMessage)) return ServiceResult<LoanDto>.Fail(errorMessage);

		if (_fineService.HasUnpaidFines(loan.UserId))
			return ServiceResult<LoanDto>.Fail(ValidationMessages.UserHasUnpaidFines);

		loan.Renew();
		_loanRepository.Update(loan);
		return ServiceResult<LoanDto>.Ok(loan.ToDto(), ValidationMessages.RenewedSuccessfully);
	}


	public IReadOnlyList<LoanDto> GetOverdueLoans(ICurrentUserSession session)
	{
		if (session.IsSelfServiceMember)
			return _loanRepository.GetActiveLoansByUser(session.UserId!.Value).Where(l => l.IsOverdue)
				.Select(loan => loan.ToDto())
				.ToList().AsReadOnly();

		return _loanRepository.GetOverdueLoans().Select(loan => loan.ToDto()).ToList().AsReadOnly();
	}


	public ServiceResult<IReadOnlyList<LoanDto>> GetLoansByUser(int userId, ICurrentUserSession session)
	{
		if (session.IsSelfServiceMember && session.UserId != userId)
			return ServiceResult<IReadOnlyList<LoanDto>>.Fail(ValidationMessages.ViewOwnLoans);

		var loans = _loanRepository.GetAllByUser(userId).Select(loan => loan.ToDto()).ToList().AsReadOnly();
		return ServiceResult<IReadOnlyList<LoanDto>>.Ok(loans, "Loans retrieved successfully.");
	}


	public IReadOnlyList<LoanDto> SearchLoans<T>(T? searchTerm, Func<LoanDto, T?> selector, Func<T, T, bool> comparer,
		ICurrentUserSession session) where T : class
	{
		if (searchTerm is null) return [];
		// TODO	(EF)	When EF is added, move search filtering to ILoanRepository.Search<T>() to allow SQL-level filtering instead of in-memory LINQ.

		return SearchLoansInternal(searchTerm, selector, comparer, session, activeOnly: false);
	}


	public IReadOnlyList<LoanDto> SearchLoans<T>(T? searchTerm, Func<LoanDto, T?> selector, Func<T, T, bool> comparer,
		ICurrentUserSession session) where T : struct
	{
		return !searchTerm.HasValue
			? []
			: SearchLoansInternal(searchTerm.Value, selector, comparer, session, activeOnly: false);
	}


	public IReadOnlyList<LoanDto> SearchActiveLoans<T>(T? searchTerm, Func<LoanDto, T?> selector,
		Func<T, T, bool> comparer, ICurrentUserSession session) where T : class
	{
		return searchTerm is null ? [] : SearchLoansInternal(searchTerm, selector, comparer, session, activeOnly: true);
	}


	public IReadOnlyList<LoanDto> SearchActiveLoans<T>(T? searchTerm, Func<LoanDto, T?> selector,
		Func<T, T, bool> comparer, ICurrentUserSession session) where T : struct
	{
		return !searchTerm.HasValue
			? []
			: SearchLoansInternal(searchTerm.Value, selector, comparer, session, activeOnly: true);
	}


	private IReadOnlyList<LoanDto> SearchLoansInternal<T>(T searchTerm, Func<LoanDto, T?> selector,
		Func<T, T, bool> comparer, ICurrentUserSession session, bool activeOnly) where T : class
	{
		IEnumerable<Loan> source = session.IsSelfServiceMember
			? _loanRepository.GetActiveLoansByUser(session.UserId!.Value)
			: activeOnly
				? _loanRepository.GetActiveLoans()
				: _loanRepository.GetAll();

		return source
			.Where(loan =>
			{
				var value = selector(loan.ToDto());
				return value is not null && comparer(searchTerm, value);
			}).Select(loan => loan.ToDto()).ToList().AsReadOnly();
	}


	private IReadOnlyList<LoanDto> SearchLoansInternal<T>(T searchTerm, Func<LoanDto, T?> selector,
		Func<T, T, bool> comparer, ICurrentUserSession session, bool activeOnly) where T : struct
	{
		IEnumerable<Loan> source = session.IsSelfServiceMember
			? _loanRepository.GetActiveLoansByUser(session.UserId!.Value)
			: activeOnly
				? _loanRepository.GetActiveLoans()
				: _loanRepository.GetAll();

		return source
			.Where(loan =>
			{
				var value = selector(loan.ToDto());
				return value.HasValue && comparer(searchTerm, value.Value);
			}).Select(loan => loan.ToDto()).ToList().AsReadOnly();
	}


	public IReadOnlyList<LoanDto> GetAllLoans(ICurrentUserSession session)
	{
		if (session.IsSelfServiceMember)
		{
			if (session.UserId is null) return [];
			return _loanRepository.GetAllByUser(session.UserId!.Value).Select(loan => loan.ToDto()).ToList().AsReadOnly();
		}

		return _loanRepository.GetAll().Select(loan => loan.ToDto()).ToList().AsReadOnly();
	}


	public IReadOnlyList<LoanDto> GetLoanByBook(int bookId)
	{
		return _loanRepository.GetLoansByBook(bookId).Select(loan => loan.ToDto()).ToList().AsReadOnly();
	}


	public IReadOnlyList<LoanDto> GetAllActiveLoans(ICurrentUserSession session)
	{
		if (session.IsSelfServiceMember)
		{
			if (session.UserId is null) return [];
			return _loanRepository.GetActiveLoansByUser(session.UserId!.Value).Select(loan => loan.ToDto()).ToList().AsReadOnly();
		}

		return _loanRepository.GetActiveLoans().Select(loan => loan.ToDto()).ToList().AsReadOnly();
	}
}