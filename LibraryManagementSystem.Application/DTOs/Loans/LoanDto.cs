namespace LibraryManagementSystem.Application.DTOs.Loans;

public class LoanDto
{
	public int LoanId { get; init; }
	public string BookName { get; init; } = null!;
	public DateOnly BorrowDate { get; init; }
	public DateOnly DueDate { get; init; }
	public DateOnly? ReturnDate { get; init; }
	public string Status { get; init; } = null!;
	public int RenewalCount { get; init; }
	public bool IsOverdue { get; init; }
	public DateTime CreatedAt { get; init; }
	public DateTime? UpdatedAt { get; init; }
}