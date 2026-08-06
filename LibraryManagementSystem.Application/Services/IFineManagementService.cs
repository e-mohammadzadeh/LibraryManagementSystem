using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Fine;

namespace LibraryManagementSystem.Application.Services;

public interface IFineManagementService
{
	ServiceResult<FineDto> CreateFineForLoan(int loanId);
	ServiceResult<FineDto> PayFine(int fineId, ICurrentUserSession session);
	ServiceResult<FineDto> WaiveFine(int fineId, ICurrentUserSession session);
	IReadOnlyList<FineDto> GetAllFines(ICurrentUserSession session);
	IReadOnlyList<FineDto> GetAllUnpaidFines(ICurrentUserSession session);
	IReadOnlyList<FineDto> GetFinesByUser(int userId);
	IReadOnlyList<FineDto> GetUnpaidFinesByUser(int userId);
	decimal GetTotalUnpaidAmount(int userId);
	bool HasUnpaidFines(int userId);
}