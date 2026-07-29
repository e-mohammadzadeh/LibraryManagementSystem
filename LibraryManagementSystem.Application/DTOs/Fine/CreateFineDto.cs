namespace LibraryManagementSystem.Application.DTOs.Fine;

public class CreateFineDto
{
	public required int LoanId { get; init; }
	public string? Reason { get; init; }
}