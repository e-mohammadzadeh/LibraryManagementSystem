namespace LibraryManagementSystem.Application.DTOs.Loans;

public class CreateLoanDto
{
	public required int UserId { get; set; }
	public required int BookId { get; set; }
}