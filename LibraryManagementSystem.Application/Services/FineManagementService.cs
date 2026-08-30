using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Fine;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class FineManagementService : IFineManagementService
{
	private readonly IFineRepository _fineRepository;
	private readonly ILoanRepository _loanRepository;
	private readonly IUserRepository _userRepository;
	private readonly IUserAutoRemovalService _userAutoRemovalService;
	private readonly IAuthorizationService _authorization;
	private readonly ILoanHistoryManagementService _loanHistoryManagementService;


	public FineManagementService(IFineRepository fineRepository, ILoanRepository loanRepository,
		IUserRepository userRepository, IUserAutoRemovalService userAutoRemovalService,
		IAuthorizationService authorization, ILoanHistoryManagementService loanHistoryManagementService)
	{
		_fineRepository = fineRepository;
		_loanRepository = loanRepository;
		_userRepository = userRepository;
		_userAutoRemovalService = userAutoRemovalService;
		_authorization = authorization;
		_loanHistoryManagementService = loanHistoryManagementService;
	}


	public ServiceResult<FineDto> CreateFineForLoan(int loanId)
	{
		var loan = _loanRepository.FindById(loanId);
		if (loan is null) return ServiceResult<FineDto>.Fail(Messages.NotLoanMatched);

		if (loan.ReturnDate is null) return ServiceResult<FineDto>.Fail(Messages.LoanNotYetReturned);

		if (loan.ReturnDate <= loan.DueDate) return ServiceResult<FineDto>.Fail(Messages.NoFine);

		var existing = _fineRepository.GetByLoanId(loanId);
		if (existing.Count > 0) return ServiceResult<FineDto>.Fail(Messages.FineAlreadyExists);

		var fine = new Fine(loan);
		_fineRepository.Add(fine);

		var totalUnpaid = _fineRepository.GetTotalUnpaidAmount(loan.UserId);
		if (fine.Amount >= ValidationConstants.MaxUnpaidFineThreshold ||
		    totalUnpaid >= ValidationConstants.MaxUnpaidFineThreshold)
		{
			var user = _userRepository.FindById(loan.UserId);
			if (user is not null && !user.ShouldRemove)
			{
				user.FlagForRemoval();
				_userRepository.Update(user);
				return ServiceResult<FineDto>.Warning(fine.ToDto(),
					string.Format(Messages.UserEligibleForRemoval, user.FirstName, user.LastName));
			}
		}

		return ServiceResult<FineDto>.Ok(fine.ToDto(), Messages.FineCreatedSuccessfully);
	}



	public ServiceResult<FineDto> PayFine(int fineId, ICurrentUserSession session)
	{
		var fine = _fineRepository.FindById(fineId);
		if (fine is null) return ServiceResult<FineDto>.Fail(Messages.FineNotFound);

		if (session.IsSelfServiceMember && session.UserId != fine.UserId)
			return ServiceResult<FineDto>.Fail(Messages.CanPayOwnFine);
		try
		{
			fine.Pay();
			_fineRepository.Update(fine);
			
			var removalResult = _userAutoRemovalService.TryAutoRemove(fine.UserId);
			_loanHistoryManagementService.Record(fine.Loan, LoanHistoryAction.FinePaid);
			var message = Messages.FinePaidSuccessfully;
			if (removalResult.Success) message = $"{message} | {removalResult.Message}";
			return ServiceResult<FineDto>.Ok(fine.ToDto(), message);
		}
		catch (InvalidOperationException ex)
		{
			return ServiceResult<FineDto>.Fail(ex.Message);
		}
	}


	public ServiceResult<FineDto> WaiveFine(int fineId)
	{
		if (!_authorization.HasPermission(Permission.WaiveFine))
			return ServiceResult<FineDto>.Fail(Messages.AdminOnlyWaive);

		var fine = _fineRepository.FindById(fineId);
		if (fine is null) return ServiceResult<FineDto>.Fail(Messages.FineNotFound);

		try
		{
			fine.Waive();
			_fineRepository.Update(fine);
			_loanHistoryManagementService.Record(fine.Loan, LoanHistoryAction.FineWaived);

			var removalResult = _userAutoRemovalService.TryAutoRemove(fine.UserId);
			var message = Messages.FineWaivedSuccessfully;
			if (removalResult.Success) message = $"{message} | {removalResult.Message}";

			return ServiceResult<FineDto>.Ok(fine.ToDto(), message);
		}
		catch (InvalidOperationException ex)
		{
			return ServiceResult<FineDto>.Fail(ex.Message);
		}
	}


	public IReadOnlyList<FineDto> GetAllUnpaidFines(ICurrentUserSession session)
	{
		return session.IsSelfServiceMember
			? GetUnpaidFinesByUser(session.UserId!.Value)
			: [.. _fineRepository.GetAllUnpaid().Select(f => f.ToDto())];
	}


	public IReadOnlyList<FineDto> GetFinesByUser(int userId) =>
		[.. _fineRepository.GetByUserId(userId).Select(fine => fine.ToDto())];


	public IReadOnlyList<FineDto> GetUnpaidFinesByUser(int userId) =>
		[.. _fineRepository.GetUnpaidByUserId(userId).Select(fine => fine.ToDto())];


	public decimal GetTotalUnpaidAmount(int userId) => _fineRepository.GetTotalUnpaidAmount(userId);

	public bool HasUnpaidFines(int userId) => _fineRepository.HasUnpaidFines(userId);


	public IReadOnlyList<FineDto> GetFineHistory()
	{
		if (!_authorization.HasPermission(Permission.ViewFineHistory)) return [];

		return [.. _fineRepository.GetHistory().Select(fine => fine.ToDto())];
	}


	public IReadOnlyList<FineDto> GetFineHistoryByUser(int userId, ICurrentUserSession session)
	{
		if ((session.IsSelfServiceMember && session.UserId != userId) ||
		    !_authorization.HasAnyPermission(Permission.FineHistoryByUser, Permission.ViewFineHistory))
			return [];

		return [.. _fineRepository.GetHistoryByUserId(userId).Select(fine => fine.ToDto())];
	}
}