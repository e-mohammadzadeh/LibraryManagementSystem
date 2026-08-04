using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.DTOs.Loans;

public class LoanDto
{
	public int LoanId { get; init; }
	public string BookName { get; init; } = null!;
	public int BookId { get; init; }
	public string BookISBN { get; init; } = null!;
	public string UserName { get; init; } = null!;
	public int UserId { get; init; }
	public string UserNationalCode { get; init; } = null!;
	public DateOnly BorrowDate { get; init; }
	public DateOnly DueDate { get; init; }
	public DateOnly? ReturnDate { get; init; }
	public LoanStatus Status { get; init; }
	public int RenewalCount { get; init; }
	public bool IsOverdue { get; init; }
	public DateTime CreatedAt { get; init; }
	public DateTime? UpdatedAt { get; init; }
}