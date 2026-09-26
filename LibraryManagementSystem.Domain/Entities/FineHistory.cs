using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.ValueObjects;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class FineHistory
{
	public Guid Id { get; set; }
	public Fine Fine { get; set; } = null!;
	public Guid FineId { get; set; }
	public Loan Loan { get; set; } = null!;
	public Guid LoanId { get; set; }
	public User User { get; set; } = null!;
	public Guid UserId { get; set; }
	public int OverdueDays { get; set; }
	public Money Money{ get; set; } = null!;
	public FineStatus Status { get; set; }
	public FineHistoryAction Action { get; set; }
	public DateTime OccurredAt { get; set; }
	public string? Description { get; set; }
}