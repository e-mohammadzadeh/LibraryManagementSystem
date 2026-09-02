namespace LibraryManagementSystem.Domain.Enums;

public static class FineHistoryActionExtensions
{
	public static string GetDefaultDescription(this FineHistoryAction action)
	{
		return action switch
		{
			FineHistoryAction.FinePaid => "Fine Paid.",
			FineHistoryAction.FineWaived => "Fine waived.",
			_ => "Action performed."
		};
	}
}