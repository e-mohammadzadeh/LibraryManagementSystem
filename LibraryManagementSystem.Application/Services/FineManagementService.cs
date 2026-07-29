using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Fine;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class FineManagementService
{
	private readonly IFineRepository _fineRepository;
	private readonly ILoanRepository _loanRepository;
	private readonly IUserRepository _userRepository;


	public FineManagementService(IFineRepository fineRepository, ILoanRepository loanRepository,
		IUserRepository userRepository)
	{
		_fineRepository = fineRepository;
		_loanRepository = loanRepository;
		_userRepository = userRepository;
	}


	public ServiceResult<FineDto> CreateFineForLoan(int loanId)
	{
		var loan = _loanRepository.FindById(loanId);
		if (loan is null) return ServiceResult<FineDto>.Fail(ValidationMessages.NotLoanMatched);

		if (loan.ReturnDate is null) return ServiceResult<FineDto>.Fail(ValidationMessages.NotReturned);

		if (loan.ReturnDate <= loan.DueDate)
			return ServiceResult<FineDto>.Fail(ValidationMessages.NoFine);

		var overdueDays = loan.ReturnDate.Value.DayNumber - loan.DueDate.DayNumber;

		var existing = _fineRepository.GetByLoanId(loanId);
		if (existing.Count > 0) return ServiceResult<FineDto>.Fail(ValidationMessages.ExistedFine);

		var fine = new Fine(loan, overdueDays);
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

		return ServiceResult<FineDto>.Ok(MapToDto(fine), ValidationMessages.FineCreatedSuccessfully);
	}


	public ServiceResult<FineDto> PayFine(int fineId)
	{
		var fine = _fineRepository.FindById(fineId);
		if (fine is null) return ServiceResult<FineDto>.Fail(ValidationMessages.FineNotFound);

		try
		{
			fine.Pay();
			_fineRepository.Update(fine);
			return ServiceResult<FineDto>.Ok(MapToDto(fine), ValidationMessages.FinePaidSuccessfully);
		}
		catch (InvalidOperationException ex)
		{
			return ServiceResult<FineDto>.Fail(ex.Message);	
		}
	}


	public ServiceResult<FineDto> WaiveFine(int fineId)
	{
		var fine = _fineRepository.FindById(fineId);
		if (fine is null) return ServiceResult<FineDto>.Fail(ValidationMessages.FineNotFound);

		try
		{
			fine.Waive();
			_fineRepository.Update(fine);
			return ServiceResult<FineDto>.Ok(MapToDto(fine), ValidationMessages.FineWaivedSuccessfully);
		}
		catch (InvalidOperationException ex)
		{
			return ServiceResult<FineDto>.Fail(ex.Message);
		}
	}


	public IReadOnlyList<FineDto> GetAllFines() => _fineRepository.GetAll().Select(MapToDto).ToList().AsReadOnly();


	public IReadOnlyList<FineDto> GetAllUnpaidFines() =>
		_fineRepository.GetAllUnpaid().Select(MapToDto).ToList().AsReadOnly();


	public IReadOnlyList<FineDto> GetFinesByUser(int userId) =>
		_fineRepository.GetByUserId(userId).Select(MapToDto).ToList().AsReadOnly();


	public IReadOnlyList<FineDto> GetUnpaidFinesByUser(int userId) =>
		_fineRepository.GetUnpaidByUserId(userId).Select(MapToDto).ToList().AsReadOnly();


	public decimal GetTotalUnpaidAmount(int userId) => _fineRepository.GetTotalUnpaidAmount(userId);

	public bool HasUnpaidFines(int userId) => _fineRepository.HasUnpaidFines(userId);


	private static FineDto MapToDto(Fine fine) 
	{
		return new FineDto
		{
			FineId = fine.FineId,
			LoanId = fine.LoanId,
			UserId = fine.UserId,
			UserFullName = fine.Loan?.User is not null ? $"{fine.Loan.User.FirstName} {fine.Loan.User.LastName}" : "Unknown",
			BookName = fine.Loan?.Book?.BookName ?? "Unknown",
			OverdueDays = fine.OverdueDays,
			Amount = fine.Amount,
			Status = fine.Status,
			Reason = fine.Reason,
			CreatedAt = fine.CreatedAt,
			UpdatedAt = fine.UpdatedAt,
			PaidAt = fine.PaidAt
		};
	}
}