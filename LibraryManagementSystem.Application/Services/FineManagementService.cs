using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Fine;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class FineManagementService : IFineManagementService
{
	private readonly IFineRepository _fineRepository;
	private readonly ILoanRepository _loanRepository;
	private readonly IUserRepository _userRepository;
	private readonly UserAutoRemovalService _userAutoRemovalService;


	public FineManagementService(IFineRepository fineRepository, ILoanRepository loanRepository,
		IUserRepository userRepository, UserAutoRemovalService userAutoRemovalService)
	{
		_fineRepository = fineRepository;
		_loanRepository = loanRepository;
		_userRepository = userRepository;
		_userAutoRemovalService = userAutoRemovalService;
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
			var message = Messages.FinePaidSuccessfully;
			if (removalResult.Success) message += $" | {removalResult.Message}";

			return ServiceResult<FineDto>.Ok(fine.ToDto(), message);
		}
		catch (InvalidOperationException ex)
		{
			return ServiceResult<FineDto>.Fail(ex.Message);
		}
	}


	public ServiceResult<FineDto> WaiveFine(int fineId, ICurrentUserSession session)
	{
		if (!session.IsAdmin) return ServiceResult<FineDto>.Fail(Messages.AdminOnlyWaive);

		var fine = _fineRepository.FindById(fineId);
		if (fine is null) return ServiceResult<FineDto>.Fail(Messages.FineNotFound);

		try
		{
			fine.Waive();
			_fineRepository.Update(fine);

			var removalResult = _userAutoRemovalService.TryAutoRemove(fine.UserId);
			var message = Messages.FineWaivedSuccessfully;
			if (removalResult.Success) message += $" | {removalResult.Message}";

			return ServiceResult<FineDto>.Ok(fine.ToDto(), message);
		}
		catch (InvalidOperationException ex)
		{
			return ServiceResult<FineDto>.Fail(ex.Message);
		}
	}


	public IReadOnlyList<FineDto> GetAllFines(ICurrentUserSession session)
	{
		if (session.IsSelfServiceMember) return GetFinesByUser(session.UserId!.Value);

		return _fineRepository.GetAll().Select(f => f.ToDto()).ToList().AsReadOnly();
	}


	public IReadOnlyList<FineDto> GetAllUnpaidFines(ICurrentUserSession session)
	{
		if (session.IsSelfServiceMember) return GetUnpaidFinesByUser(session.UserId!.Value);

		return _fineRepository.GetAllUnpaid().Select(f => f.ToDto()).ToList().AsReadOnly();
	}


	public IReadOnlyList<FineDto> GetFinesByUser(int userId) =>
		_fineRepository.GetByUserId(userId).Select(fine => fine.ToDto()).ToList().AsReadOnly();


	public IReadOnlyList<FineDto> GetUnpaidFinesByUser(int userId) =>
		_fineRepository.GetUnpaidByUserId(userId).Select(fine => fine.ToDto()).ToList().AsReadOnly();


	public decimal GetTotalUnpaidAmount(int userId) => _fineRepository.GetTotalUnpaidAmount(userId);

	public bool HasUnpaidFines(int userId) => _fineRepository.HasUnpaidFines(userId);
}