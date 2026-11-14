using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Application.Seeders;

public static class AdminSeeder
{
	public static void EnsureAdmin(UserManagementService userService)	
	{
		var admin = new User
		{
			FirstName = "Sara",
			LastName = "Admin",
			NationalCode = "2790806801",
			Email = Email.Create("admin@library.com"),
			PhoneNumber = PhoneNumber.Create("09120000010"),
			BirthDate = new DateOnly(1985, 3, 15),
			IsActive = true,
			ShouldRemove = false,
			MembershipStartDate = DateOnly.FromDateTime(DateTime.Today),
			MembershipExpiryDate = new DateOnly(2099, 12, 31),
		};

		var result = userService.CreateInitialAdmin(admin, "Admin@123");

		if (!result.IsSuccess)
			throw new InvalidOperationException("Initial Admin creation failed.");
	}
}