using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Infrastructure.DTOs.Fine;

public class FineDto
{
	public Guid? FineId { get; init; }
	public Guid? LoanId { get; init; }
	public Guid? UserId { get; init; }
	public string? UserFullName { get; init; } = null!;
	public string? BookName { get; init; } = null!;
	public int? OverdueDays { get; init; }
	public Money? Money { get; init; } = null!;
	public FineStatus? Status { get; init; }
	public string? Reason { get; init; } = null!;
	public DateTime? CreatedAt { get; init; }
	public DateTime? UpdatedAt { get; init; }
	public DateOnly? PaidAt { get; init; }
}