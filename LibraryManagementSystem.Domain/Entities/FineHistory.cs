using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Domain.Entities;

public class FineHistory
{
	public Guid Id { get; set; }
	public Guid FineId { get; set; }
	public Guid LoanId { get; set; }
	public Guid UserId { get; set; }
	public int OverdueDays { get; set; }
	public Money Money{ get; set; } = null!;
	public FineStatus Status { get; set; }
	public FineHistoryAction Action { get; set; }
	public DateTime OccurredAt { get; set; }
	public string? Description { get; set; }
}