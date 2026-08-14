namespace LibraryManagementSystem.Application.DTOs.Fine;

public class CreateFineDto
{
	// TODO	Use it when there are other reasons to create fine not only just for overdue days. {reason: Damage book, ...}
	public required int LoanId { get; init; }
	public string? Reason { get; init; }
}