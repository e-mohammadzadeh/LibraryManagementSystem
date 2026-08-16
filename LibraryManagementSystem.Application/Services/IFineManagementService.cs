using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Fine;

namespace LibraryManagementSystem.Application.Services;

public interface IFineManagementService
{
	ServiceResult<FineDto> CreateFineForLoan(int loanId);
	ServiceResult<FineDto> PayFine(int fineId, ICurrentUserSession session);
	ServiceResult<FineDto> WaiveFine(int fineId);
	IReadOnlyList<FineDto> GetAllUnpaidFines(ICurrentUserSession session);
	IReadOnlyList<FineDto> GetFinesByUser(int userId);
	IReadOnlyList<FineDto> GetUnpaidFinesByUser(int userId);
	decimal GetTotalUnpaidAmount(int userId);
	bool HasUnpaidFines(int userId);
	IReadOnlyList<FineDto> GetFineHistory();
	IReadOnlyList<FineDto> GetFineHistoryByUser(int userId, ICurrentUserSession session);
}