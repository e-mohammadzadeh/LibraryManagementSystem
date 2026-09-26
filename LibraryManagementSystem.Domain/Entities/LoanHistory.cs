using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class LoanHistory
{
	public Guid Id { get; set; }
	public Loan Loan { get; set; } = null!;
	public Guid LoanId { get; set; }
	public User User { get; set; } = null!;
	public Guid UserId { get; set; }
	public Book Book { get; set; } = null!;
	public Guid BookId { get; set; }
	public LoanHistoryAction Action { get; set; }
	public DateTime OccurredAt { get; set; }
	public string? Description { get; set; }
}