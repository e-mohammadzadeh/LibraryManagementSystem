namespace LibraryManagementSystem.Application.DTOs.Fine;

public class CreateFineDto
{
	public required int LoanId { get; init; }
	public required int UserId { get; init; }
	public required int OverdueDays { get; init; }
	public required DateOnly DueDate { get; init; }
	public required DateOnly ReturnDate { get; init; }
	public string? Reason { get; init; }
}