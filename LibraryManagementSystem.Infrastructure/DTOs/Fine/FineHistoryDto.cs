using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Infrastructure.DTOs.Fine;

public class FineHistoryDto
{
	public Guid Id { get; init; }
	public Guid FineId { get; init; }
	public Guid LoanId { get; init; }
	public Guid UserId { get; init; }
	public string UserName { get; init; } = string.Empty;
	public FineHistoryAction Action { get; init; }
	public string ActionDisplay => Action.GetDefaultDescription();
	public DateTime OccurredAt { get; init; }
	public decimal Amount { get; init; }
	public string? Description { get; init; }
}