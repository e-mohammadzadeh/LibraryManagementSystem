using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

internal static class PersonHelper
{
	internal static void SearchAndDisplay<TEntity>(string prompt, Func<string, IReadOnlyList<TEntity>> searchFn,
		Action<IReadOnlyList<TEntity>> printFn, string noResultsMessage)
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



	internal static void PerformRemove<TEntity>(TEntity? entity, string firstName, string lastName,
		Action<TEntity> printDetailsFn, Func<ServiceResult<TEntity>> removeFn) where TEntity : class
	{
		if (entity is null) return;

		printDetailsFn(entity);
		var choice = ConsoleHelper.ReadYesNo($"Are you sure you want to remove {firstName} {lastName}");

		if (choice != true) return;

		var result = removeFn();
		ConsoleHelper.ShowResult(result);
	}
}