using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Infrastructure.DTOs.Fine;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Application.Services;

public interface IFineManagementService
{
	ServiceResult<FineDto> CreateFineForLoan(Guid loanId);
	ServiceResult<FineDto> PayFine(Guid fineId, ICurrentUserSession session);
	ServiceResult<FineDto> WaiveFine(Guid fineId);
	IReadOnlyList<FineDto> GetAllUnpaidFines(ICurrentUserSession session);
	IReadOnlyList<FineDto> GetFinesByUser(Guid userId, FineFilter filter);
	bool HasUnpaidFines(Guid userId);
	IReadOnlyList<FineDto> GetFineHistory();
	IReadOnlyList<FineDto> GetFineHistoryByUser(Guid userId, ICurrentUserSession session);
}