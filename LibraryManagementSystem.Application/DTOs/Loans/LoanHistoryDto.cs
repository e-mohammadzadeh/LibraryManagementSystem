using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.DTOs.Loans;

public class LoanHistoryDto
{
	public int Id { get; init; }
	public int LoanId { get; init; }
	public int UserId { get; init; }
	public int BookId { get; init; }

	public string UserName { get; init; } = string.Empty;
	public string BookName { get; init; } = string.Empty;

	public LoanHistoryAction Action { get; init; }

	public DateTime OccurredAt { get; init; }

	public string? Description { get; init; }
}