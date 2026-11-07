namespace LibraryManagementSystem.Infrastructure.Enums.Filters;

public enum FineFilter
{
	All,      // All fines (including unpaid)
	Paid,     // Only paid fines
	Unpaid,   // Only unpaid fines
	Waived,   // Only waived fines
}