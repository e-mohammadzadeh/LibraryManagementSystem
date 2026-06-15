namespace LibraryManagementSystem;

public class Loan
{
	public int LoanId { get; set; }
	public Member Member { get; set; }
	public Book Book { get; set; }
	public DateOnly BorrowDate { get; set; }
	public DateOnly DueDate { get; set; }
	public DateOnly? ReutrnDate { get; set; }
}