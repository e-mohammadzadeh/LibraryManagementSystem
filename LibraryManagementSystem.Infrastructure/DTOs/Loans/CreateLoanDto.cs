namespace LibraryManagementSystem.Infrastructure.DTOs.Loans;

public class CreateLoanDto
{
	public required int UserId { get; init; }
	public required int BookId { get; init; }
}