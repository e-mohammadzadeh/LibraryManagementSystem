using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Loans;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Application.Authorization;

namespace LibraryManagementSystem.Application.Services;

public class LoanManagementService
{
	private readonly ILoanRepository _loanRepository;
	private readonly IUserRepository _userRepository;
	private readonly IBookRepository _bookRepository;
	private readonly IFineManagementService _fineService;
	private readonly IAuthorizationService _authorization;
	private readonly ILoanHistoryManagementService _loanHistoryManagementService;


	public LoanManagementService(ILoanRepository loanRepository, IUserRepository userRepository,
		IBookRepository bookRepository, IFineManagementService fineManagementService,
		IAuthorizationService authorization, ILoanHistoryManagementService loanHistoryManagementService)
	{
		_loanRepository = loanRepository;
		_userRepository = userRepository;
		_bookRepository = bookRepository;
		_fineService = fineManagementService;
		_authorization = authorization;
		_loanHistoryManagementService = loanHistoryManagementService;
	}


	public ServiceResult<LoanDto> BorrowBook(CreateLoanDto dto, ICurrentUserSession session)
	{
		if (session.IsSelfServiceMember && session.UserId != dto.UserId)
			return ServiceResult<LoanDto>.Fail(Messages.BorrowBookForYourself);

		var user = _userRepository.FindById(dto.UserId);
		if (user is null) return ServiceResult<LoanDto>.Fail(Messages.NotUserMatched);

		if (!user.IsActive) return ServiceResult<LoanDto>.Fail(Messages.MembershipExpired);

		if (user.ShouldRemove) return ServiceResult<LoanDto>.Fail(Messages.FlaggedForRemoval);

		if (_fineService.HasUnpaidFines(dto.UserId)) return ServiceResult<LoanDto>.Fail(Messages.BorrowFailedForFine);

		if (_loanRepository.CountActiveLoansByUser(dto.UserId) >= ValidationConstants.MaxActiveLoansPerUser)
			return ServiceResult<LoanDto>.Fail(Messages.MaximumLoansReached);

		var book = _bookRepository.FindById(dto.BookId);
		if (book is null) return ServiceResult<LoanDto>.Fail(Messages.NotBookMatched);

		if (book.AvailableCopies <= 0) return ServiceResult<LoanDto>.Fail(Messages.NotEnoughCopiesAvailable);

		if (_loanRepository.HasActiveLoan(dto.UserId, dto.BookId))
			return ServiceResult<LoanDto>.Fail(Messages.BookAlreadyBorrowed);

		var loan = new Loan(book, user, DateOnly.FromDateTime(DateTime.Today));
		book.BorrowCopy();
		_loanRepository.Add(loan);
		_bookRepository.Update(book);
		_loanHistoryManagementService.Record(loan, LoanHistoryAction.Borrowed);

		return ServiceResult<LoanDto>.Ok(loan.ToDto(), Messages.BorrowedSuccessfully);
	}


	public ServiceResult<LoanDto> ReturnBook(int loanId, ICurrentUserSession session)
	{
		var loan = _loanRepository.GetActiveLoanById(loanId);
		if (loan is null) return ServiceResult<LoanDto>.Fail(Messages.ActiveLoanNotFound);

		if (session.IsSelfServiceMember && session.UserId != loan.UserId)
			return ServiceResult<LoanDto>.Fail(Messages.ReturnOwnLoans);

		loan.MarkAsReturned();
		loan.Book.ReturnCopy();
		_loanRepository.Update(loan);
		_bookRepository.Update(loan.Book);
		_loanHistoryManagementService.Record(loan, LoanHistoryAction.Returned);

		var fineResult = _fineService.CreateFineForLoan(loanId);
		if (!fineResult.Success && fineResult.Message != Messages.NoFine)
			return ServiceResult<LoanDto>.Warning(loan.ToDto(),
				$"{Messages.ReturnedSuccessfully} - Note: {fineResult.Message}");

		return ServiceResult<LoanDto>.Ok(loan.ToDto(), Messages.ReturnedSuccessfully);
	}


	public ServiceResult<IReadOnlyList<LoanDto>> GetActiveLoansByUser(int userId, ICurrentUserSession session)
	{
		if (session.IsSelfServiceMember && session.UserId != userId)
			return ServiceResult<IReadOnlyList<LoanDto>>.Fail(Messages.ViewOwnLoans);

		IReadOnlyList<LoanDto> loans = [.. _loanRepository.GetActiveLoansByUser(userId).Select(loan => loan.ToDto())];
		return ServiceResult<IReadOnlyList<LoanDto>>.Ok(loans, Messages.LoansRetrievedSuccessfully);
	}


	public ServiceResult<LoanDto> RenewLoan(int loanId, ICurrentUserSession session)
	{
		var loan = _loanRepository.GetActiveLoanById(loanId);
		if (loan is null) return ServiceResult<LoanDto>.Fail(Messages.ActiveLoanNotFound);

		if (session.IsSelfServiceMember && session.UserId != loan.UserId)
			return ServiceResult<LoanDto>.Fail(Messages.RenewOwnLoans);

		if (!loan.CanRenew(out var errorMessage)) return ServiceResult<LoanDto>.Fail(errorMessage);

		if (_fineService.HasUnpaidFines(loan.UserId)) return ServiceResult<LoanDto>.Fail(Messages.UserHasUnpaidFines);

		loan.Renew();
		_loanRepository.Update(loan);
		_loanHistoryManagementService.Record(loan, LoanHistoryAction.Renewed);

		return ServiceResult<LoanDto>.Ok(loan.ToDto(), Messages.RenewedSuccessfully);
	}


	public ServiceResult<IReadOnlyList<LoanDto>> GetOverdueLoansByUser(int userId, ICurrentUserSession session)
	{
		if (session.IsSelfServiceMember && session.UserId != userId)
			return ServiceResult<IReadOnlyList<LoanDto>>.Fail(Messages.ViewOwnLoans);

		List<LoanDto> loans =
		[
			.. _loanRepository.GetActiveLoansByUser(userId).Where(loan => loan.IsOverdue).Select(loan => loan.ToDto())
		];

		return ServiceResult<IReadOnlyList<LoanDto>>.Ok(loans, Messages.LoansRetrievedSuccessfully);
	}


	public IReadOnlyList<LoanDto> GetOverdueLoansByBook(int bookId, ICurrentUserSession session)
	{
		if (session.IsSelfServiceMember) return [];

		return
		[
			.. _loanRepository.GetActiveLoansByBook(bookId).Where(loan => loan.IsOverdue).Select(loan => loan.ToDto())
		];
	}


	public IReadOnlyList<LoanDto> GetOverdueLoans(ICurrentUserSession session)
	{
		if (session is { IsSelfServiceMember: true, UserId: not null })
			return
			[
				.. _loanRepository.GetActiveLoansByUser(session.UserId.Value).Where(l => l.IsOverdue)
					.Select(loan => loan.ToDto())
			];

		return [.. _loanRepository.GetOverdueLoans().Select(loan => loan.ToDto())];
	}


	public IReadOnlyList<LoanDto> GetLoansByUser(int userId, ICurrentUserSession session)
	{
		if (session.IsSelfServiceMember && session.UserId != userId) return [];
		return [.. _loanRepository.GetAllByUser(userId).Select(loan => loan.ToDto())];
	}


	public IReadOnlyList<LoanDto> SearchLoans<T>(T? searchTerm, Func<Loan, T?> selector, Func<T, T, bool> comparer,
		ICurrentUserSession session) where T : class
	{
		// TODO	(EF)	When EF is added, move search filtering to ILoanRepository.Search<T>() to allow SQL-level filtering instead of in-memory LINQ.
		return searchTerm is null
			? []
			: SearchLoansInternal(searchTerm, selector, comparer, session, activeOnly: false);
	}


	public IReadOnlyList<LoanDto> SearchLoans<T>(T? searchTerm, Func<Loan, T?> selector, Func<T, T, bool> comparer,
		ICurrentUserSession session) where T : struct
	{
		return !searchTerm.HasValue
			? []
			: SearchLoansInternal(searchTerm.Value, selector, comparer, session, activeOnly: false);
	}


	public IReadOnlyList<LoanDto> SearchActiveLoans<T>(T? searchTerm, Func<Loan, T?> selector,
		Func<T, T, bool> comparer, ICurrentUserSession session) where T : class
	{
		return searchTerm is null ? [] : SearchLoansInternal(searchTerm, selector, comparer, session, activeOnly: true);
	}


	public IReadOnlyList<LoanDto> SearchActiveLoans<T>(T? searchTerm, Func<Loan, T?> selector,
		Func<T, T, bool> comparer, ICurrentUserSession session) where T : struct
	{
		return !searchTerm.HasValue
			? []
			: SearchLoansInternal(searchTerm.Value, selector, comparer, session, activeOnly: true);
	}


	private IReadOnlyList<LoanDto> SearchLoansInternal<T>(T searchTerm, Func<Loan, T?> selector,
		Func<T, T, bool> comparer, ICurrentUserSession session, bool activeOnly) where T : class
	{
		if (session.UserId is null) return [];

		IEnumerable<Loan> source = session.IsSelfServiceMember
			? (activeOnly
				? _loanRepository.GetActiveLoansByUser(session.UserId.Value)
				: _loanRepository.GetAllByUser(session.UserId.Value))
			: (activeOnly ? _loanRepository.GetActiveLoans() : _loanRepository.GetAll());

		return
		[
			.. source
				.Where(loan =>
				{
					var value = selector(loan);
					return value is not null && comparer(searchTerm, value);
				}).Select(loan => loan.ToDto())
		];
	}


	private IReadOnlyList<LoanDto> SearchLoansInternal<T>(T searchTerm, Func<Loan, T?> selector,
		Func<T, T, bool> comparer, ICurrentUserSession session, bool activeOnly) where T : struct
	{
		if (session.UserId is null) return [];
		IEnumerable<Loan> source = session.IsSelfServiceMember
			? (activeOnly
				? _loanRepository.GetActiveLoansByUser(session.UserId.Value)
				: _loanRepository.GetAllByUser(session.UserId.Value))
			: (activeOnly ? _loanRepository.GetActiveLoans() : _loanRepository.GetAll());

		return
		[
			.. source
				.Where(loan =>
				{
					var value = selector(loan);
					return value.HasValue && comparer(searchTerm, value.Value);
				}).Select(loan => loan.ToDto())
		];
	}


	public IReadOnlyList<LoanDto> GetAllLoans(ICurrentUserSession session)
	{
		if (session is { IsSelfServiceMember: true, UserId: not null })
			return [.. _loanRepository.GetAllByUser(session.UserId.Value).Select(loan => loan.ToDto())];

		return [.. _loanRepository.GetAll().Select(loan => loan.ToDto())];
	}


	public IReadOnlyList<LoanDto> GetLoanByBook(int bookId, ICurrentUserSession session)
	{
		if (session.IsSelfServiceMember) return [];
		return [.. _loanRepository.GetLoansByBook(bookId).Select(loan => loan.ToDto())];
	}


	public IReadOnlyList<LoanDto> GetOwnLoansByBook(int bookId, ICurrentUserSession session)
	{
		if (!session.IsAuthenticated || session.UserId is null) return [];
		return [.._loanRepository.GetLoansByBookAndUser(bookId, session.UserId.Value).Select(loan => loan.ToDto())];
	}


	public IReadOnlyList<LoanDto> GetActiveLoansByBook(int bookId, ICurrentUserSession session)
	{
		if (session.IsSelfServiceMember) return [];
		return [.. _loanRepository.GetActiveLoansByBook(bookId).Select(loan => loan.ToDto())];
	}


	public IReadOnlyList<LoanDto> GetAllActiveLoans(ICurrentUserSession session)
	{
		if (_authorization.HasPermission(Permission.MyActiveLoans))
		{
			return session.UserId is null
				? []
				: [.. _loanRepository.GetActiveLoansByUser(session.UserId.Value).Select(loan => loan.ToDto())];
		}

		return [.. _loanRepository.GetActiveLoans().Select(loan => loan.ToDto())];
	}


	public IReadOnlyList<LoanDto> GetFullLibraryHistory()
	{
		return [.. _loanRepository.GetAll().Select(loan => loan.ToDto())];
	}
}