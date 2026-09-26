using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class Loan
{
	public Guid LoanId { get; set; }
	public Book Book { get; set; } = null!;
	public Guid BookId { get; set; }
	public User User { get; set; } = null!;
	public Guid UserId { get; set; }
	public DateOnly BorrowDate { get; set; }
	public DateOnly DueDate { get; set; }
	public DateOnly? ReturnDate { get; set; }
	public LoanStatus Status { get; set; }
	public int RenewalCount { get; set; }
	public bool IsOverdue { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
}