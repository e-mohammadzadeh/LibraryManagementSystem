using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Search;
using LibraryManagementSystem.Infrastructure.Interfaces;
using LibraryManagementSystem.Infrastructure.ValueObjects;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public abstract class ContributorMenu
{
	public static ContributorDto? PromptForContributorDto(string contributorType, bool hasBio = true)
	{
		var label = string.IsNullOrWhiteSpace(contributorType) ? "" : $"{contributorType}'s ";

		var firstName = ConsoleHelper.GetValidName($"Enter {label}first name", ValidationConstants.MinNameLength,
			ValidationConstants.MaxNameLength);
		if (firstName is null) return null;

		var lastName = ConsoleHelper.GetValidName($"Enter {label}last name", ValidationConstants.MinNameLength,
			ValidationConstants.MaxNameLength);
		if (lastName is null) return null;

		var nationalCode = ConsoleHelper.GetValidNationalCode($"Enter {label}national code");
		if (nationalCode is null) return null;

		var email = ConsoleHelper.GetValidEmail($"Enter {label}email");
		if (email is null) return null;

		var phoneNumber = ConsoleHelper.GetValidPhoneNumber("Enter phone number");
		if (phoneNumber is null) return null;

		var birthDate = ConsoleHelper.GetValidBirthDate($"Enter {label}birth date");
		if (birthDate is null) return null;

		if (!hasBio)
			return new ContributorDto
			{
				FirstName = firstName,
				LastName = lastName,
				NationalCode = nationalCode,
				Email = Email.Create(email),
				PhoneNumber = PhoneNumber.Create(phoneNumber),
				BirthDate = birthDate.Value
			};

		var biography = ConsoleHelper.ReadString("You can add a biography (Optional)", true);
		return new ContributorDto
		{
			FirstName = firstName,
			LastName = lastName,
			NationalCode = nationalCode,
			Email = Email.Create(email),
			PhoneNumber = PhoneNumber.Create(phoneNumber),
			BirthDate = birthDate.Value,
			Biography = biography
		};
	}


	public static void EditContributor(ContributorDto contributor, string menuTitle, string menuIdPrefix,
		IMenuService menuService, Func<ContributorDto, Guid?, ServiceResult<ContributorDto>> updateContributor,
		Guid updatedBy)
	{
		while (true)
		{
			Console.Clear();
			ContributorPrinter.PrintDetails(contributor, menuTitle);

			var selectedItem = MenuHelper.DisplayMenu(menuTitle, menuService.GetChildren($"{menuIdPrefix}.edit"));
			switch (selectedItem)
			{
				case null:
					return;

				case var _ when selectedItem == $"{menuIdPrefix}.edit.back":
					ConsoleHelper.ShowInfo(string.Format(Messages.EditCancelled,
						menuTitle.Replace(" MENU", "", StringComparison.OrdinalIgnoreCase)));
					return;
			}

			var updated = true;
			switch (selectedItem)
			{
				case var _ when selectedItem == $"{menuIdPrefix}.edit.firstname":
				{
					var value = ConsoleHelper.GetValidName("\nEnter new first name", ValidationConstants.MinNameLength,
						ValidationConstants.MaxNameLength);

					if (value != null)
						contributor.FirstName = value;
					else
						updated = false;

					break;
				}
				case var _ when selectedItem == $"{menuIdPrefix}.edit.lastname":
				{
					var value = ConsoleHelper.GetValidName("\nEnter new last name", ValidationConstants.MinNameLength,
						ValidationConstants.MaxNameLength);

					if (value != null)
						contributor.LastName = value;
					else
						updated = false;

					break;
				}
				case var _ when selectedItem == $"{menuIdPrefix}.edit.national-code":
				{
					var value = ConsoleHelper.GetValidNationalCode("\nEnter new national code");

					if (value != null)
						contributor.NationalCode = value;
					else
						updated = false;

					break;
				}
				case var _ when selectedItem == $"{menuIdPrefix}.edit.email":
				{
					var value = ConsoleHelper.GetValidEmail("\nEnter new email");

					if (value != null)
						contributor.Email = Email.Create(value);
					else
						updated = false;

					break;
				}

				case var _ when selectedItem == $"{menuIdPrefix}.edit.phone-number":
				{
					var value = ConsoleHelper.GetValidPhoneNumber("\nEnter new phone number");

					if (value != null)
						contributor.PhoneNumber = PhoneNumber.Create(value);
					else
						updated = false;

					break;
				}

				case var _ when selectedItem == $"{menuIdPrefix}.edit.birthdate":
				{
					var value = ConsoleHelper.GetValidBirthDate("\nEnter new birth date");

					if (value != null)
						contributor.BirthDate = value.Value;
					else
						updated = false;

					break;
				}

				case var _ when selectedItem == $"{menuIdPrefix}.edit.biography":
				{
					var value = ConsoleHelper.ReadString("\nEnter new biography");

					if (value != null)
						contributor.Biography = value;
					else
						updated = false;

					break;
				}

				default:
				{
					updated = false;
					break;
				}
			}

			if (updated)
			{
				var result = updateContributor(contributor, updatedBy);
				ConsoleHelper.ShowResult(result);
			}

			var choice = ConsoleHelper.ReadYesNo(Messages.EditContinuesQuestion);
			if (choice != true) return;
		}
	}


	public static void RemoveContributor<T>(T? contributor, string firstName, string lastName,
		Action<T> printDetailsFn, Func<ServiceResult<T>> removeFn) where T : class
	{
		if (contributor is null) return;

		printDetailsFn(contributor);
		var choice = ConsoleHelper.ReadYesNo($"Are you sure you want to remove {firstName} {lastName}");

		if (choice != true) return;

		var result = removeFn();
		ConsoleHelper.ShowResult(result);
	}


	public static void SearchContributor(IReadOnlyList<ContributorDto> contributors, string contributorName,
		string menuIdPrefix, IMenuService menuService, IAuthorizationService authorization,
		Func<string, ContributorSearchField, IReadOnlyList<ContributorDto>> searchContributor, string notMatchedMessage)
	{
		if (contributors.Count == 0)
		{
			ConsoleHelper.ShowWarning(contributorName == "Author"
				? Messages.NotAvailableAuthor
				: Messages.NotAvailableTranslator);
			return;
		}

		Action<IReadOnlyList<ContributorDto>> printer = authorization.HasAnyPermission(
			Permission.ViewContributorDetails,
			Permission.FullSearchContributor)
			? results => ContributorPrinter.PrintFullTable(results, "Search Result")
			: results => ContributorPrinter.PrintTable(results, "Search Result");

		while (true)
		{
			Console.Clear();

			var selectedItem = MenuHelper.DisplayMenu($"SEARCHING {contributorName.ToUpper()} MENU",
				menuService.GetChildren($"{menuIdPrefix}.search"));

			switch (selectedItem)
			{
				case null:
					return;

				case var _ when selectedItem == $"{menuIdPrefix}.search.back":
					ConsoleHelper.ShowInfo(string.Format(Messages.SearchCancelled, contributorName));
					return;
			}

			var field = selectedItem switch
			{
				_ when selectedItem == $"{menuIdPrefix}.search.name" => ContributorSearchField.Name,
				_ when selectedItem == $"{menuIdPrefix}.search.national-code" => ContributorSearchField.NationalCode,
				_ when selectedItem == $"{menuIdPrefix}.search.email" => ContributorSearchField.Email,
				_ when selectedItem == $"{menuIdPrefix}.search.phone-number" => ContributorSearchField.PhoneNumber,
				_ => (ContributorSearchField?)null
			};

			if (field is null) continue;

			var prompt = field.Value switch
			{
				ContributorSearchField.Name => Messages.SearchName,
				ContributorSearchField.NationalCode => Messages.SearchNationalCode,
				ContributorSearchField.Email => Messages.SearchEmail,
				ContributorSearchField.PhoneNumber => Messages.SearchPhoneNumber,
				_ => "Enter search term"
			};

			SearchAndDisplay(prompt, term => searchContributor(term, field.Value), printer,
				notMatchedMessage);
			ConsoleHelper.Pause();
		}
	}


	public static void SearchAndDisplay<T>(string prompt, Func<string, IReadOnlyList<T>> searchFn,
		Action<IReadOnlyList<T>> printFn, string noResultsMessage)
	{
		Console.Clear();
		var searchItem = ConsoleHelper.ReadString(prompt);
		if (searchItem is null) return;

		var result = searchFn(searchItem);
		if (result.Count == 0)
		{
			ConsoleHelper.ShowWarning(noResultsMessage);
			return;
		}

		printFn(result);
	}


	public static void ViewContributors(
		string contributorName,
		string menuIdPrefix,
		string backMessage,
		string unavailableMessage,
		string unavailableRemovedMessage,
		IMenuService menuService,
		IAuthorizationService authorization,
		ICurrentUserSession session,
		LibraryStatisticsService statisticsService,
		Func<IReadOnlyList<ContributorDto>> getAllContributors,
		Func<IReadOnlyList<ContributorDto>> getRemovedContributors,
		Func<ContributorDto?> selectContributor,
		Action viewBooks,
		bool requireViewPermission = false)
	{
		if (requireViewPermission &&
		    !authorization.HasAnyPermission(
			    Permission.ViewContributorDetails,
			    Permission.ViewContributorBooks,
			    Permission.ViewAllContributors))
			return;

		var contributorNameLower = contributorName.ToLowerInvariant();
		var pluralName = $"{contributorNameLower}s";

		while (true)
		{
			ConsoleHelper.ClearConsole();

			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);

			var selectedItem = MenuHelper.DisplayMenu($"VIEW {contributorName.ToUpperInvariant()} MENU",
				menuService.GetChildren($"{menuIdPrefix}.view"));

			switch (selectedItem)
			{
				case null:
					return;

				case var _ when selectedItem == $"{menuIdPrefix}.view.back":
					ConsoleHelper.ShowInfo(backMessage);
					return;

				case var _ when selectedItem == $"{menuIdPrefix}.view.{contributorNameLower}-details":
				{
					if (!authorization.HasPermission(Permission.ViewContributorDetails)) break;

					Console.Clear();

					var contributor = selectContributor();

					if (contributor is not null) ContributorPrinter.PrintDetails(contributor);

					ConsoleHelper.Pause();
					break;
				}

				case var _ when selectedItem == $"{menuIdPrefix}.view.{contributorNameLower}-books":
				{
					Console.Clear();
					viewBooks();
					ConsoleHelper.Pause();
					break;
				}

				case var _ when selectedItem == $"{menuIdPrefix}.view.all-{pluralName}":
				{
					if (!authorization.HasPermission(Permission.ViewAllContributors)) break;

					Console.Clear();

					var contributors = getAllContributors();

					if (contributors.Count == 0)
						ConsoleHelper.ShowWarning(unavailableMessage);
					else if (authorization.HasPermission(Permission.ViewContributorDetails))
						ContributorPrinter.PrintFullTable(contributors);
					else
						ContributorPrinter.PrintTable(contributors);

					ConsoleHelper.Pause();
					break;
				}

				case var _ when selectedItem == $"{menuIdPrefix}.view.removed-{pluralName}":
				{
					if (!authorization.HasPermission(Permission.ViewRemovedContributors)) break;

					Console.Clear();

					var removedContributors = getRemovedContributors();

					if (removedContributors.Count == 0)
						ConsoleHelper.ShowWarning(unavailableRemovedMessage);
					else
						ContributorPrinter.PrintFullTable(removedContributors);

					ConsoleHelper.Pause();
					break;
				}
			}
		}
	}
}