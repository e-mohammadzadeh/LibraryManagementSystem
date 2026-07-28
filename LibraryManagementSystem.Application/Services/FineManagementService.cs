namespace LibraryManagementSystem.Application.Services;

public class FineManagementService {
	// check whether loan is overdue
	// public bool IsOverdue =>!ReturnDate.HasValue && DateOnly.FromDateTime(DateTime.Today) > DueDate;

	// calculate fine
	// OverdueDays = Today - DueDate(in days, minimum 1)
	// TotalAmount = OverdueDays × DailyRate
	// TotalAmount = Math.Min(TotalAmount, MaxFineAmount)  // optional cap

	// create fine


	// ===============

	//	GetAllFines() → IReadOnlyList<FineDto>
	//	GetUnpaidFines() → IReadOnlyList<FineDto>
	//	GetFinesByUser(int userId) → IReadOnlyList<FineDto>
	//	GetTotalUnpaidAmount(int userId) → decimal
	//	PayFine(int fineId) → ServiceResult<FineDto>
	//	WaiveFine(int fineId) → ServiceResult<FineDto>
}