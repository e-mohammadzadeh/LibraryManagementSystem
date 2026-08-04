using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Fine;

namespace LibraryManagementSystem.Application.Services;

public interface IFineManagementService
{
	ServiceResult<FineDto> CreateFineForLoan(int loanId);
	ServiceResult<FineDto> PayFine(int fineId);
	ServiceResult<FineDto> WaiveFine(int fineId);
	IReadOnlyList<FineDto> GetAllFines();
	IReadOnlyList<FineDto> GetAllUnpaidFines();
	IReadOnlyList<FineDto> GetFinesByUser(int userId);
	IReadOnlyList<FineDto> GetUnpaidFinesByUser(int userId);
	decimal GetTotalUnpaidAmount(int userId);
	bool HasUnpaidFines(int userId);
}