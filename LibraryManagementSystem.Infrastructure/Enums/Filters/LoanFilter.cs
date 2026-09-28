namespace LibraryManagementSystem.Infrastructure.Enums.Filters;

public enum LoanFilter
{
	All,      // All loans (including removed)
	Active,   // Only active loans (not removed) – default
	Inactive, // Only inactive loans (removed)
	Overdue,  // Only overdue loans
	Returned, // Only returned loans
}