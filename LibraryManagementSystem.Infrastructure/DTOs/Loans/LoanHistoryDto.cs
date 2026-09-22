using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Infrastructure.DTOs.Loans;

public class LoanHistoryDto
{
	public Guid Id { get; init; }
	public Guid LoanId { get; init; }
	public Guid UserId { get; init; }
	public Guid BookId { get; init; }

	public string UserName { get; init; } = string.Empty;
	public string BookName { get; init; } = string.Empty;

	// TODO (ASP.NET Core): Convert to string for API responses
	public LoanHistoryAction Action { get; init; }
	public string ActionDisplay => Action.GetDefaultDescription();

	public DateTime OccurredAt { get; init; }

	public string? Description { get; init; }
}