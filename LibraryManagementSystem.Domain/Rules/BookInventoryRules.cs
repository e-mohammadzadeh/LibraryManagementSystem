namespace LibraryManagementSystem.Domain.Rules;

public class BookInventoryRules
{
	public static bool CanChangeTotalCopies(int totalCopies, int availableCopies, int newTotalCopies)
	{
		if (newTotalCopies <= 0) return false;

		var unavailableCopies = totalCopies - availableCopies;

		return newTotalCopies >= unavailableCopies;
	}
}