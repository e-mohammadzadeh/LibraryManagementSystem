namespace LibraryManagementSystem.Domain.Enums;

public static class LoanHistoryActionExtensions
{
	public static string GetDefaultDescription(this LoanHistoryAction action)
	{
		return action switch
		{
			LoanHistoryAction.Borrowed => "Book borrowed.",
			LoanHistoryAction.Returned => "Book returned.",
			LoanHistoryAction.Renewed => "Loan renewed.",
			LoanHistoryAction.Lost => "Book marked as lost.",
			LoanHistoryAction.PaidFine => "Fine paid.",
			_ => "Action performed."
		};
	}
}