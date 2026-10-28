using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Rules;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Loans;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class LoanManagementService
{
	private readonly ILoanRepository _loanRepository;
	private readonly IUserRepository _userRepository;
	private readonly IBookRepository _bookRepository;
	private readonly IBookManagementService _bookService;
	private readonly IFineRepository _fineRepository;
	private readonly IFineManagementService _fineManagementService;
	private readonly IUserAutoRemovalService _userAutoRemovalService;
	private readonly IAuthorizationService _authorization;
	private readonly IAuditLogManagementService _auditLog;


	public LoanManagementService(ILoanRepository loanRepository, IUserRepository userRepository,
		IBookRepository bookRepository, IBookManagementService bookService,
		IFineRepository fineRepository, IFineManagementService fineManagementService,
		IUserAutoRemovalService userAutoRemovalService, IAuthorizationService authorization,
		IAuditLogManagementService auditLog)
	{
		_loanRepository = loanRepository;
		_userRepository = userRepository;
		_bookRepository = bookRepository;
		_bookService = bookService;
		_fineRepository = fineRepository;
		_fineManagementService = fineManagementService;
		_userAutoRemovalService = userAutoRemovalService;
		_authorization = authorization;
		_auditLog = auditLog;
	}


	public ServiceResult<LoanDto> BorrowBook(LoanDto dto, ICurrentUserSession session)
	{
		if (session.IsSelfServiceMember && session.CurrentUser.Id != dto.UserId)
			return ServiceResult<LoanDto>.Fail(Messages.BorrowBookForYourself);

		var user = _userRepository.GetById(dto.UserId, EntityFilter.Active);
		if (user is null) return ServiceResult<LoanDto>.Fail(Messages.NotUserMatched);

		if (!user.IsActive) return ServiceResult<LoanDto>.Fail(Messages.MembershipExpired);

		if (user.ShouldRemove)
			return ServiceResult<LoanDto>.Fail(string.Format(Messages.FlaggedForRemoval, "Borrowing"));

		if (_fineRepository.HasFines(dto.UserId, FineFilter.Unpaid))
			return ServiceResult<LoanDto>.Fail(Messages.BorrowFailedForFine);

		if (CountLoans(dto.UserId, LoanFilter.Active) >= ValidationConstants.MaxActiveLoansPerUser)
			return ServiceResult<LoanDto>.Fail(Messages.MaximumLoansReached);

		if (_loanRepository.GetByUserId(dto.UserId, LoanFilter.Active).Any(l => l.IsOverdue))
			return ServiceResult<LoanDto>.Fail(Messages.BorrowBlockedDueToOverdue);

		var book = _bookRepository.GetById(dto.BookId, EntityFilter.Active);
		if (book is null) return ServiceResult<LoanDto>.Fail(Messages.NotBookMatched);

		if (book.AvailableCopies <= 0) return ServiceResult<LoanDto>.Fail(Messages.NotEnoughCopiesAvailable);

		if (_loanRepository.GetLoansByBookAndUser(dto.BookId, dto.UserId, LoanFilter.Active).Count > 0)
			return ServiceResult<LoanDto>.Fail(Messages.BookAlreadyBorrowed);

		var loan = new Loan
		{
			Book = book,
			BookId = book.Id,
			User = user,
			UserId = user.Id
		};

		_bookService.BorrowCopy(dto.BookId);
		_loanRepository.Add(loan);
		_bookRepository.Update(book);

		_auditLog.Record(AuditAction.LoanBorrowed, "Loan", loan.Id, "Loan borrowed.");

		return ServiceResult<LoanDto>.Ok(loan.ToDto(), Messages.BorrowedSuccessfully);
	}


	public ServiceResult<LoanDto> ReturnBook(Guid loanId, ICurrentUserSession session)
	{
		var loan = _loanRepository.GetById(loanId, LoanFilter.Active);
		if (loan is null) return ServiceResult<LoanDto>.Fail(Messages.ActiveLoanNotFound);

		if (session.IsSelfServiceMember && session.CurrentUser.Id != loan.UserId)
			return ServiceResult<LoanDto>.Fail(Messages.ReturnOwnLoans);

		MarkAsReturned(loanId);
		_bookService.ReturnCopy(loan.BookId);
		_loanRepository.Update(loan);
		_bookRepository.Update(loan.Book);

		_userAutoRemovalService.TryAutoRemove(loan.UserId);
		_auditLog.Record(AuditAction.LoanReturned, "Loan", loanId, "Loan returned.");

		var fineResult = _fineManagementService.CreateFineForLoan(loanId);
		if (!fineResult.Success && fineResult.Message != Messages.NoFine)
			return ServiceResult<LoanDto>.Warning(loan.ToDto(),
				$"{Messages.ReturnedSuccessfully} - Note: {fineResult.Message}");

		return ServiceResult<LoanDto>.Ok(loan.ToDto(), Messages.ReturnedSuccessfully);
	}


	public ServiceResult<LoanDto> RenewLoan(LoanDto dto, ICurrentUserSession session)
	{
		var loan = _loanRepository.GetById(dto.Id, LoanFilter.Active);
		if (loan is null) return ServiceResult<LoanDto>.Fail(Messages.ActiveLoanNotFound);

		if (session.IsSelfServiceMember && session.CurrentUser.Id != loan.UserId)
			return ServiceResult<LoanDto>.Fail(Messages.RenewOwnLoans);

		if (loan.User.ShouldRemove)
			return ServiceResult<LoanDto>.Fail(string.Format(Messages.FlaggedForRemoval, "Renewing"));


		if (!LoanRenewalRules.CanRenew(loan, out var errorMessage)) return ServiceResult<LoanDto>.Fail(errorMessage);

		if (_fineRepository.HasFines(loan.UserId, FineFilter.Unpaid))
			return ServiceResult<LoanDto>.Fail(Messages.UserHasUnpaidFines);

		ReturnBook(loan.Id, session);
		_loanRepository.Update(loan);
		BorrowBook(dto, session);
		_loanRepository.Update(loan);

		return ServiceResult<LoanDto>.Ok(loan.ToDto(), Messages.RenewedSuccessfully);
	}


	public IReadOnlyList<LoanDto> GetAllLoans(ICurrentUserSession session, LoanFilter filter)
	{
		if (session.IsSelfServiceMember)
			return [.. _loanRepository.GetByUserId(session.CurrentUser.Id, filter).Select(loan => loan.ToDto())];

		return [.. _loanRepository.GetAll(filter).Select(loan => loan.ToDto())];
	}


	public IReadOnlyList<LoanDto> GetLoansByUser(Guid userId, ICurrentUserSession session, LoanFilter filter)
	{
		if (session.IsSelfServiceMember && session.CurrentUser.Id != userId) return [];
		return [.. _loanRepository.GetByUserId(userId, filter).Select(loan => loan.ToDto())];
	}


	public IReadOnlyList<LoanDto> GetLoanByBook(Guid bookId, ICurrentUserSession session, LoanFilter filter)
	{
		if (session.IsSelfServiceMember) return [];
		return [.. _loanRepository.GetByBookId(bookId, filter).Select(loan => loan.ToDto())];
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
		IEnumerable<Loan> source = session.IsSelfServiceMember
			? _loanRepository.GetByUserId(session.CurrentUser.Id, filter)
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
		IEnumerable<Loan> source = session.IsSelfServiceMember
			? _loanRepository.GetByUserId(session.CurrentUser.Id, filter)
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
		if (!session.IsAuthenticated) return [];
		return
		[
			.._loanRepository.GetLoansByBookAndUser(bookId, session.CurrentUser.Id, LoanFilter.All)
				.Select(loan => loan.ToDto())
		];
	}


	private void MarkAsReturned(Guid loanId, DateOnly? returnDate = null)
	{
		var loan = _loanRepository.GetById(loanId, LoanFilter.Active);
		if (loan is null) return;

		if (loan.ReturnDate is not null) throw new InvalidOperationException("This loan has already been returned.");

		loan.ReturnDate = returnDate ?? DateOnly.FromDateTime(DateTime.Today);
		loan.Status = LoanStatus.Returned;
	}


	private int CountLoans(Guid? userId, LoanFilter filter)
	{
		return _loanRepository.GetByUserId(userId!.Value, filter).Count;
	}
}