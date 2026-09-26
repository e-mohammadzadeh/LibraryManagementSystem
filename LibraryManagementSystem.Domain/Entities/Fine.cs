using LibraryManagementSystem.Domain.ValueObjects;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Fine
{
	public Guid FineId { get; set; }
	public Guid LoanId { get; set; }
	public Loan Loan { get; set; } = null!;
	public Guid UserId { get; set; }
	public int OverdueDays { get; set; }
	public Money Money{ get; set; } = null!;
	public FineStatus Status { get; set; }
	public string Reason { get; set; } = string.Empty;
	public decimal DailyRate { get;  set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public DateOnly? PaidAt { get; set; }
}