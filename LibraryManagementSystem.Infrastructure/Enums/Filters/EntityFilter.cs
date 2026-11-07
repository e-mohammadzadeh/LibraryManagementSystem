namespace LibraryManagementSystem.Infrastructure.Enums.Filters;

public enum EntityFilter
{
	All,      // All users (including removed)
	Active,   // Only active users (not removed) – default
	Removed   // Only removed users
}