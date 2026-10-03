using LibraryManagementSystem.Domain.ValueObjects;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Fine
{
	public Guid Id { get; set; }
	public Loan Loan { get; set; } = null!;
	public Guid LoanId { get; set; }
	public Guid UserId { get; set; }
	public int OverdueDays { get; set; }
	public Money Money { get; set; } = null!;
	public FineStatus Status { get; set; }
	public DateOnly? PaidAt { get; set; }
	public string Reason { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public Guid? UpdatedByUserId { get; set; }
}