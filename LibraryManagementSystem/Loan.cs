namespace LibraryManagementSystem;

public class Loan
{
	public int LoanId { get; set; }
	public Book Book { get; set; }
	public DateOnly BorrowDate { get; set; }
	public DateOnly DueDate { get; set; }
	public DateOnly? ReturnDate { get; set; }

	public bool IsOverdue
	{
		get;
		set => IsOverdue = !ReturnDate.HasValue && DateOnly.FromDateTime(DateTime.Today) > DueDate;
	}
}