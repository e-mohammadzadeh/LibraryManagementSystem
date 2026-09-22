namespace LibraryManagementSystem.Infrastructure.DTOs.Loans;

public class CreateLoanDto
{
	public required Guid UserId { get; init; }
	public required Guid BookId { get; init; }
}