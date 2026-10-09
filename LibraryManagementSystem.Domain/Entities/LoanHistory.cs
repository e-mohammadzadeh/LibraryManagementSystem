using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class LoanHistory
{
	public Guid Id { get; set; }
	public Guid LoanId { get; set; }
	public Guid UserId { get; set; }
	public Guid BookId { get; set; }
	public LoanHistoryAction Action { get; set; }
	public DateTime OccurredAt { get; set; }
	public string? Description { get; set; }
}