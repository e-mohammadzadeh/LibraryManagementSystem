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
			LoanHistoryAction.FinePaid => "Fine paid.",
			LoanHistoryAction.FineWaived => "Fine waived by admin.",
			_ => "Action performed."
		};
	}
}