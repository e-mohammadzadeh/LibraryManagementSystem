using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.DTOs.Fine;

public class FineHistoryDto
{
	public int Id { get; init; }
	public int FineId { get; init; }
	public int LoanId { get; init; }
	public int UserId { get; init; }
	public FineHistoryAction Action { get; init; }
	public string ActionDisplay => Action.GetDefaultDescription();
	public DateTime OccurredAt { get; init; }
	public string? Description { get; init; }
}