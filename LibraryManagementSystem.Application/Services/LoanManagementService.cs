using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Loans;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class LoanManagementService : ILoanManagementService
{
	private readonly ILoanRepository _loanRepository;
	private readonly IUserRepository _userRepository;
	private readonly IBookRepository _bookRepository;
	private readonly IFineManagementService _fineService;
	private readonly IUserAutoRemovalService _userAutoRemovalService;
	private readonly IAuthorizationService _authorization;
	private readonly ILoanHistoryManagementService _loanHistoryManagementService;
	private readonly IAuditLogManagementService _auditLog;


	public LoanManagementService(ILoanRepository loanRepository, IUserRepository userRepository,
		IBookRepository bookRepository, IFineManagementService fineManagementService,
		IUserAutoRemovalService userAutoRemovalService, IAuthorizationService authorization,
		ILoanHistoryManagementService loanHistoryManagementService, IAuditLogManagementService auditLog)
	{
		_loanRepository = loanRepository;
		_userRepository = userRepository;
		_bookRepository = bookRepository;
		_fineService = fineManagementService;
		_userAutoRemovalService = userAutoRemovalService;
		_authorization = authorization;
		_loanHistoryManagementService = loanHistoryManagementService;
		_auditLog = auditLog;
	}


	public ServiceResult<LoanDto> BorrowBook(CreateLoanDto dto, ICurrentUserSession session)
	{
		if (session.IsSelfServiceMember && session.UserId != dto.UserId)
			return ServiceResult<LoanDto>.Fail(Messages.BorrowBookForYourself);

		var user = _userRepository.FindById(dto.UserId, EntityFilter.Active);
		if (user is null) return ServiceResult<LoanDto>.Fail(Messages.NotUserMatched);

		if (!user.IsActive) return ServiceResult<LoanDto>.Fail(Messages.MembershipExpired);

		if (user.ShouldRemove)
			return ServiceResult<LoanDto>.Fail(string.Format(Messages.FlaggedForRemoval, "Borrowing"));

		if (_fineService.HasUnpaidFines(dto.UserId)) return ServiceResult<LoanDto>.Fail(Messages.BorrowFailedForFine);

		if (_loanRepository.CountLoans(dto.UserId, LoanFilter.Active) >= ValidationConstants.MaxActiveLoansPerUser)
			return ServiceResult<LoanDto>.Fail(Messages.MaximumLoansReached);

		if (_loanRepository.GetAllByUser(dto.UserId, LoanFilter.Active).Any(l => l.IsOverdue))
			return ServiceResult<LoanDto>.Fail(Messages.BorrowBlockedDueToOverdue);

		var book = _bookRepository.FindById(dto.BookId, EntityFilter.Active);
		if (book is null) return ServiceResult<LoanDto>.Fail(Messages.NotBookMatched);

		if (book.AvailableCopies <= 0) return ServiceResult<LoanDto>.Fail(Messages.NotEnoughCopiesAvailable);

		if (_loanRepository.HasLoans(dto.UserId, dto.BookId, LoanFilter.Active))
			return ServiceResult<LoanDto>.Fail(Messages.BookAlreadyBorrowed);

		var loan = new Loan
		{
			Book = book,
			BookId = book.Id,
			User = user,
			UserId = user.Id
		};

		_bookRepository.BorrowCopy(book);
		_loanRepository.Add(loan);
		_bookRepository.Update(book, session.UserId);
		_loanHistoryManagementService.Record(loan, LoanHistoryAction.Borrowed);
		_auditLog.Record(AuditAction.LoanBorrowed, "Loan", loan.Id, "Loan borrowed.");

		return ServiceResult<LoanDto>.Ok(loan.ToDto(), Messages.BorrowedSuccessfully);
	}


	public ServiceResult<LoanDto> ReturnBook(Guid loanId, ICurrentUserSession session)
	{
		var loan = _loanRepository.FindById(loanId, LoanFilter.Active);
		if (loan is null) return ServiceResult<LoanDto>.Fail(Messages.ActiveLoanNotFound);

		if (session.IsSelfServiceMember && session.UserId != loan.UserId)
			return ServiceResult<LoanDto>.Fail(Messages.ReturnOwnLoans);

		loan.MarkAsReturned();
		_bookRepository.ReturnCopy(loan.Book);
		_loanRepository.Update(loan);
		_bookRepository.Update(loan.Book, session.UserId);
		_loanHistoryManagementService.Record(loan, LoanHistoryAction.Returned);
		_userAutoRemovalService.TryAutoRemove(loan.UserId);
		_auditLog.Record(AuditAction.LoanReturned, "Loan", loanId, "Loan returned.");

		var fineResult = _fineService.CreateFineForLoan(loanId);
		if (!fineResult.Success && fineResult.Message != Messages.NoFine)
			return ServiceResult<LoanDto>.Warning(loan.ToDto(),
				$"{Messages.ReturnedSuccessfully} - Note: {fineResult.Message}");

		return ServiceResult<LoanDto>.Ok(loan.ToDto(), Messages.ReturnedSuccessfully);
	}


	public ServiceResult<LoanDto> RenewLoan(Guid loanId, ICurrentUserSession session)
	{
		var loan = _loanRepository.FindById(loanId, LoanFilter.Active);
		if (loan is null) return ServiceResult<LoanDto>.Fail(Messages.ActiveLoanNotFound);

		if (session.IsSelfServiceMember && session.UserId != loan.UserId)
			return ServiceResult<LoanDto>.Fail(Messages.RenewOwnLoans);

		if (loan.User.ShouldRemove)
			return ServiceResult<LoanDto>.Fail(string.Format(Messages.FlaggedForRemoval, "Renewing"));


		if (!loan.CanRenew(out var errorMessage)) return ServiceResult<LoanDto>.Fail(errorMessage);

		if (_fineService.HasUnpaidFines(loan.UserId)) return ServiceResult<LoanDto>.Fail(Messages.UserHasUnpaidFines);

		loan.Renew();
		_loanRepository.Update(loan);
		_loanHistoryManagementService.Record(loan, LoanHistoryAction.Renewed);
		_auditLog.Record(AuditAction.LoanRenewed, "Loan", loanId, "Loan renewed.");

		return ServiceResult<LoanDto>.Ok(loan.ToDto(), Messages.RenewedSuccessfully);
	}


	public IReadOnlyList<LoanDto> GetAllLoans(ICurrentUserSession session, LoanFilter filter)
	{
		if ((_authorization.HasPermission(Permission.MyActiveLoans) || session.IsSelfServiceMember) &&
		    session.UserId is not null)
			return [.. _loanRepository.GetAllByUser(session.UserId.Value, filter).Select(loan => loan.ToDto())];

		return [.. _loanRepository.GetAll(filter).Select(loan => loan.ToDto())];
	}


	public IReadOnlyList<LoanDto> GetLoansByUser(Guid userId, ICurrentUserSession session, LoanFilter filter)
	{
		if (session.IsSelfServiceMember && session.UserId != userId) return [];
		return [.. _loanRepository.GetAllByUser(userId, filter).Select(loan => loan.ToDto())];
	}


	public IReadOnlyList<LoanDto> GetLoanByBook(Guid bookId, ICurrentUserSession session, LoanFilter filter)
	{
		if (session.IsSelfServiceMember) return [];
		return [.. _loanRepository.GetLoansByBook(bookId, filter).Select(loan => loan.ToDto())];
	}


	public IReadOnlyList<LoanDto> SearchLoans<T>(T? searchTerm, Func<Loan, T?> selector, Func<T, T, bool> comparer,
		ICurrentUserSession session, LoanFilter filter) where T : class
	{
		// TODO	(EF)	When EF is added, move search filtering to ILoanRepository.Search<T>() to allow SQL-level filtering instead of in-memory LINQ.
		return searchTerm is null ? [] : SearchLoansInternal(searchTerm, selector, comparer, session, filter);
	}


	public IReadOnlyList<LoanDto> SearchLoans<T>(T? searchTerm, Func<Loan, T?> selector, Func<T, T, bool> comparer,
		ICurrentUserSession session, LoanFilter filter) where T : struct
	{
		return !searchTerm.HasValue ? [] : SearchLoansInternal(searchTerm.Value, selector, comparer, session, filter);
	}


	private IReadOnlyList<LoanDto> SearchLoansInternal<T>(T searchTerm, Func<Loan, T?> selector,
		Func<T, T, bool> comparer, ICurrentUserSession session, LoanFilter filter) where T : class
	{
		if (session.UserId is null) return [];

		IEnumerable<Loan> source = session.IsSelfServiceMember
			? _loanRepository.GetAllByUser(session.UserId.Value, filter)
			: _loanRepository.GetAll(filter);

		return
		[
			.. source.Where(l =>
			{
				var value = selector(l);
				return value != null && comparer(searchTerm, value);
			}).Select(loan => loan.ToDto())
		];
	}


	private IReadOnlyList<LoanDto> SearchLoansInternal<T>(T searchTerm, Func<Loan, T?> selector,
		Func<T, T, bool> comparer, ICurrentUserSession session, LoanFilter filter) where T : struct
	{
		if (session.UserId is null) return [];

		IEnumerable<Loan> source = session.IsSelfServiceMember
			? _loanRepository.GetAllByUser(session.UserId.Value, filter)
			: _loanRepository.GetAll(filter);

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



	public IReadOnlyList<LoanDto> GetOwnLoansByBook(Guid bookId, ICurrentUserSession session)
	{
		if (!session.IsAuthenticated || session.UserId is null) return [];
		return [.._loanRepository.GetLoansByBookAndUser(bookId, session.UserId.Value, LoanFilter.All).Select(loan => loan.ToDto())];
	}


	public bool HasLoans(Guid? userId = null, Guid? bookId = null, LoanFilter filter = LoanFilter.Active) {
		if (userId is null && bookId is null)
			throw new ArgumentException("At least one of userId or bookId must be provided.");
		var query = ApplyFilter(_loans, filter);

		if (userId is not null)
			query = query.Where(l => l.UserId == userId);
		if (bookId is not null)
			query = query.Where(l => l.BookId == bookId);
		return query.Any();
	}


	public int CountLoans(Guid? userId = null, LoanFilter filter = LoanFilter.Active) {
		var query = ApplyFilter(_loans, filter);

		if (userId is not null)
			query = query.Where(l => l.UserId == userId);
		return query.Count();
	}
}