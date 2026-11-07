using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.DTOs.Books;
using LibraryManagementSystem.Infrastructure.DTOs.Contributor;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Enums.Search;
using LibraryManagementSystem.Infrastructure.Enums.Sort;
using LibraryManagementSystem.Infrastructure.Interfaces;
using LibraryManagementSystem.Infrastructure.ValueObjects;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class BookMenu
{
	public static void BookMenuController(AuthorManagementService authorManagementService,
		TranslatorManagementService translatorManagementService, BookManagementService bookManagementService,
		LoanManagementService loanManagementService, LibraryStatisticsService statisticsService,
		ICurrentUserSession session, IAuthorizationService authorization, IMenuService menuService)
	{
		if (!authorization.HasAnyPermission(Permission.AddBook,
			    Permission.EditBook,
			    Permission.RemoveBook,
			    Permission.SearchBook,
			    Permission.FullSortBook,
			    Permission.ViewBookDetails,
			    Permission.ViewAllBooks))
			return;

		while (true)
		{
			ConsoleHelper.ClearConsole();
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);
			switch (BookMenuList(menuService))
			{
				case "books.add":
				{
					if (!authorization.HasPermission(Permission.AddBook)) break;
					Console.Clear();
					AddBook(authorManagementService, translatorManagementService, bookManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case "books.edit":
				{
					if (!authorization.HasPermission(Permission.EditBook)) break;
					Console.Clear();
					EditBook(authorManagementService, menuService, translatorManagementService, bookManagementService,
						session);
					ConsoleHelper.Pause();
					break;
				}
				case "books.remove":
				{
					if (!authorization.HasPermission(Permission.RemoveBook)) break;
					Console.Clear();
					RemoveBook(bookManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case "books.search":
				{
					if (!authorization.HasPermission(Permission.SearchBook)) break;
					Console.Clear();
					SearchBook(bookManagementService, authorization, menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "books.sort":
				{
					Console.Clear();
					SortBook(bookManagementService, authorization, menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "books.view":
				{
					if (!authorization.HasAnyPermission(Permission.ViewBookDetails, Permission.ViewAllBooks)) return;
					Console.Clear();
					ViewBooks(bookManagementService, loanManagementService, authorization, session, statisticsService,
						menuService);
					ConsoleHelper.Pause();
					break;
				}
				case "books.back":
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					return;
				}
			}
		}
	}


	private static string? BookMenuList(IMenuService menuService)
	{
		var items = menuService.GetChildren("books");
		return MenuHelper.DisplayMenu("BOOK MENU", items);
	}


	private static void AddBook(AuthorManagementService authorManagementService,
		TranslatorManagementService translatorManagementService, BookManagementService bookManagementService)
	{
		Console.WriteLine(new string('=', 36) + " ADDING BOOK MENU " + new string('=', 36));

		var isbn = ConsoleHelper.ReadISBN("Enter ISBN for the new book");
		if (isbn is null) return;

		var bookName = ConsoleHelper.GetValidName("Enter the new book's full name",
			ValidationConstants.MinBookNameLength, ValidationConstants.MaxBookNameLength);
		if (bookName is null) return;

		var authorIds = ResolveAuthorIds(authorManagementService);
		if (authorIds is null) return;

		var translatorIds = ResolveTranslatorIds(translatorManagementService);
		if (translatorIds is null) return;

		var publishDate = ConsoleHelper.GetValidDate("Enter the publication date for this book");
		if (publishDate is null) return;

		var totalCopies = ConsoleHelper.ReadInt("Enter the total number of copies for this book",
			ValidationConstants.MinBookCopies, ValidationConstants.MaxBookCopies);
		if (totalCopies is null) return;

		ConsoleHelper.DisplayGenres();
		var genre = ConsoleHelper.ReadGenre("Select your desired genre by entering its ID");
		if (genre is null) return;

		ConsoleHelper.DisplayLanguages();
		var originalLanguageId = ConsoleHelper.ReadInt("Select original language of book by entering its ID", 1,
			Enum.GetValues<Language>().Length);
		if (originalLanguageId is null) return;
		var originalLanguage = (Language)originalLanguageId.Value;

		Language? translatedLanguage = null;
		if (translatorIds.Count > 0)
		{
			ConsoleHelper.DisplayLanguages();
			var translatedLanguageId = ConsoleHelper.ReadInt("Select translated language of book by entering its ID", 1,
				Enum.GetValues<Language>().Length);
			if (translatedLanguageId is null) return;
			translatedLanguage = (Language)translatedLanguageId.Value;
		}

		var publisher = ConsoleHelper.GetValidName("Enter the publisher for this book",
			ValidationConstants.MinPublisherNameLength, ValidationConstants.MaxPublisherNameLength);
		if (publisher is null) return;

		var description = ConsoleHelper.ReadString("You can add any descriptions about this book (Optional)", true);

		var result = bookManagementService.AddBook(new BookDto
		{
			Title = bookName,
			ISBN = ISBN.Create(isbn),
			Authors = authorIds,
			Translators = translatorIds,
			PublishDate = publishDate.Value,
			Genre = BookGenre.Create(genre.Value),
			Publisher = publisher,
			OriginalLanguage = originalLanguage,
			TotalCopies = totalCopies.Value,
			Description = description,
			TranslatedLanguage = translatedLanguage!.Value
		});

		ConsoleHelper.ShowResult(result);
	}


	private static List<ContributorDto>? ResolveAuthorIds(AuthorManagementService authorManagementService)
	{
		var availableAuthors = authorManagementService.GetAllAuthors();
		if (availableAuthors.Count != 0)
			return ConsoleHelper.ReadAuthorDtos("Select author(s) for this book", availableAuthors);

		var choice = ConsoleHelper.ReadYesNo(Messages.NotAvailableAuthor);
		if (choice is not true)
		{
			ConsoleHelper.ShowWarning(Messages.BookRequiresAtLeastOneAuthor);
			return null;
		}

		var createdAuthors = new List<ContributorDto>();
		while (true)
		{
			var authorDto = ContributorMenu.PromptForContributorDto("author");
			if (authorDto is null)
			{
				if (createdAuthors.Count == 0) return null;
				break;
			}

			var addResult = authorManagementService.AddAuthor(authorDto);
			ConsoleHelper.ShowResult(addResult);
			if (addResult is { Success: true, Data: not null })
				createdAuthors.Add(addResult.Data);
			else
				ConsoleHelper.ShowError(Messages.AuthorCreationFailed);

			if (createdAuthors.Count == 0) continue;

			var addMore = ConsoleHelper.ReadYesNo("Add another author for this book");
			if (addMore != true) break;
		}

		return createdAuthors.Count > 0 ? createdAuthors : null;
	}


	private static List<ContributorDto>? ResolveTranslatorIds(TranslatorManagementService translatorManagementService)
	{
		var availableTranslators = translatorManagementService.GetAllTranslators();
		if (availableTranslators.Count != 0)
			return ConsoleHelper.ReadTranslatorDtos("Select one or more translators (optional)", availableTranslators);

		var choice = ConsoleHelper.ReadYesNo(Messages.AddTranslatorInAdd);
		if (choice != true) return [];

		var createdIds = new List<ContributorDto>();
		while (true)
		{
			var translatorDto = ContributorMenu.PromptForContributorDto("translator");
			if (translatorDto is null) break;
			var addResult = translatorManagementService.AddTranslator(translatorDto);
			ConsoleHelper.ShowResult(addResult);

			if (addResult is { Success: true, Data: not null })
				createdIds.Add(addResult.Data);
			else
				ConsoleHelper.ShowError(Messages.NotAvailableTranslator);

			var addMore = ConsoleHelper.ReadYesNo("Add another translator for this book");
			if (addMore != true) break;
		}

		return createdIds;
	}


	private static void EditBook(AuthorManagementService authorManagementService, IMenuService menuService,
		TranslatorManagementService translatorManagementService, BookManagementService bookManagementService,
		ICurrentUserSession session)
	{
		var desiredBook = SelectExistingBook(bookManagementService, "EDITING BOOK MENU");
		if (desiredBook is null) return;

		while (true)
		{
			ConsoleHelper.ClearConsole();
			BookPrinter.PrintDetails(desiredBook);
			var selectedItem = MenuHelper.DisplayMenu("EDIT BOOK MENU", menuService.GetChildren("books.edit"));
			switch (selectedItem)
			{
				case null:
					return;
				case "books.edit.back":
					ConsoleHelper.ShowInfo(string.Format(Messages.EditCancelled, "Book"));
					return;
			}

			var updated = true;

			switch (selectedItem)
			{
				case "books.edit.title":
				{
					var value = ConsoleHelper.GetValidName("\nEnter the new book name",
						ValidationConstants.MinBookNameLength, ValidationConstants.MaxBookNameLength);
					if (value != null)
						desiredBook.Title = value;
					else
						updated = false;
					break;
				}
				case "books.edit.isbn":
				{
					var value = ConsoleHelper.ReadISBN("\nEnter the new ISBN");
					if (value != null)
						desiredBook.ISBN = ISBN.Create(value);
					else
						updated = false;
					break;
				}
				case "books.edit.authors":
				{
					var value = AuthorSubMenu(desiredBook.Id, authorManagementService, bookManagementService,
						menuService);
					if (value != null)
						desiredBook.Authors = value;
					else
						updated = false;
					break;
				}
				case "books.edit.translator":
				{
					var value = TranslatorSubMenu(desiredBook.Id, translatorManagementService, menuService,
						bookManagementService);
					if (value != null)
					{
						var (newTranslators, newLanguage) = value.Value;
						desiredBook.Translators = newTranslators;
						if (newLanguage != null) desiredBook.TranslatedLanguage = newLanguage.Value;
					}
					else
						updated = false;

					break;
				}
				case "books.edit.publish-date":
				{
					var value = ConsoleHelper.GetValidDate("\nEnter the new publish date");
					if (value != null)
						desiredBook.PublishDate = value.Value;
					else
						updated = false;
					break;
				}
				case "books.edit.total-copies":
				{
					var value = ConsoleHelper.ReadInt("\nEnter the new total copies", ValidationConstants.MinBookCopies,
						ValidationConstants.MaxBookCopies);
					if (value != null)
						desiredBook.TotalCopies = value.Value;
					else
						updated = false;

					break;
				}
				case "books.edit.genre":
				{
					ConsoleHelper.DisplayGenres();
					var genreId = ConsoleHelper.ReadInt("\nEnter the new genre id", 1,
						Enum.GetValues<Genre>().Length + 1);
					if (genreId != null)
						desiredBook.Genre = BookGenre.Create((Genre)(genreId - 1));
					else
						updated = false;
					break;
				}
				case "books.edit.publisher":
				{
					var value = ConsoleHelper.GetValidName("\nEnter the new publisher",
						ValidationConstants.MinPublisherNameLength, ValidationConstants.MaxPublisherNameLength);
					if (value != null)
						desiredBook.Publisher = value;
					else
						updated = false;
					break;
				}
				case "books.edit.description":
				{
					var value = ConsoleHelper.ReadString("\nEnter the new description");
					if (value != null)
						desiredBook.Description = value;
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
				var result = bookManagementService.UpdateBook(desiredBook, session.CurrentUser.Id);
				ConsoleHelper.ShowResult(result);
			}

			var choice = ConsoleHelper.ReadYesNo(Messages.EditContinuesQuestion);
			if (choice != true) return;

			var refreshedBook = bookManagementService.GetBookById(desiredBook.Id);
			if (refreshedBook is not null) desiredBook = refreshedBook;
		}
	}


	private static BookDto? SelectExistingBook(BookManagementService bookManagementService, string title)
	{
		var bookList = bookManagementService.GetAllBooks();
		if (bookList.Count is not 0) return MenuHelper.SelectBook(bookList, title);

		ConsoleHelper.ShowWarning(Messages.NotAvailableBook);
		return null;
	}


	private static List<ContributorDto>? AuthorSubMenu(Guid bookId, AuthorManagementService authorManagementService,
		BookManagementService bookManagementService, IMenuService menuService)
	{
		var currentBook = bookManagementService.GetBookById(bookId);
		if (currentBook is null)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableBook);
			return null;
		}

		var currentAuthors = currentBook.Authors;

		var items = menuService.GetChildren("books.edit.authors");
		var selectedItem = MenuHelper.DisplayMenu("AUTHOR MENU", items);
		if (selectedItem is null) return null;

		switch (selectedItem)
		{
			case "books.edit.authors.add":
			{
				var allAuthors = authorManagementService.GetAllAuthors();
				if (allAuthors.Count == 0)
				{
					ConsoleHelper.ShowError(Messages.NotAvailableAuthor);
					break;
				}

				var availableToAdd = allAuthors.Where(a => currentAuthors.All(ca => ca.Id != a.Id)).ToList()
					.AsReadOnly();
				if (availableToAdd.Count == 0)
				{
					ConsoleHelper.ShowError(Messages.NotEnoughAuthors);
					break;
				}

				var newlySelected = ConsoleHelper.ReadAuthorDtos("\nSelect author(s) for this book", availableToAdd);
				if (newlySelected is null || newlySelected.Count == 0) return null;

				// New list = existing author IDs + newly selected IDs
				return [.. currentAuthors.Concat(newlySelected).DistinctBy(a => a.Id)];
			}
			case "books.edit.authors.remove":
			{
				if (currentAuthors.Count <= 1)
				{
					ConsoleHelper.ShowWarning(Messages.CannotRemoveLastAuthor);
					break;
				}

				var selectedForRemoval = ConsoleHelper.ReadAuthorDtos(Messages.AuthorSelectionForRemove,
					currentBook.Authors, false);
				if (selectedForRemoval is null || selectedForRemoval.Count == 0) break;

				var authorToRemove = selectedForRemoval[0];
				return [.. currentAuthors.Where(a => a.Id != authorToRemove.Id)];
			}
			case "books.edit.authors.replace":
			{
				var allAuthors = authorManagementService.GetAllAuthors();
				if (allAuthors.Count == 0)
				{
					ConsoleHelper.ShowError(Messages.NotAvailableActionLoan);
					break;
				}

				var newAuthors = ConsoleHelper.ReadAuthorDtos("\nSelect the new author(s) for this book", allAuthors);
				if (newAuthors is null || newAuthors.Count == 0) break;
				return newAuthors;
			}
			case "books.edit.authors.back":
			{
				ConsoleHelper.ShowInfo("Author edit cancelled. Returning to Edit Menu...");
				break;
			}
		}

		return null;
	}


	private static (List<ContributorDto>, Language?)? TranslatorSubMenu(Guid bookId,
		TranslatorManagementService translatorManagementService, IMenuService menuService,
		BookManagementService bookManagementService)
	{
		var currentBook = bookManagementService.GetBookById(bookId);
		if (currentBook is null)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableBook);
			return null;
		}

		var currentTranslators = currentBook.Translators;

		var items = menuService.GetChildren("books.edit.translator");
		var selectedItem = MenuHelper.DisplayMenu("TRANSLATOR MENU", items);
		if (selectedItem is null) return null;

		switch (selectedItem)
		{
			case "books.edit.translator.add":
			{
				var allTranslators = translatorManagementService.GetAllTranslators();
				if (allTranslators.Count == 0)
				{
					ConsoleHelper.ShowWarning(Messages.NotAvailableTranslator);
					break;
				}

				var availableToAdd = allTranslators.Where(t => currentTranslators.All(ct => ct.Id != t.Id)).ToList()
					.AsReadOnly();
				if (availableToAdd.Count == 0)
				{
					ConsoleHelper.ShowWarning(Messages.AllTranslatorsAssigned);
					break;
				}

				var newlySelected = ConsoleHelper.ReadTranslatorDtos("\nSelect translator(s) to add", availableToAdd);
				if (newlySelected is null || newlySelected.Count == 0) break;

				Language language;
				if (currentTranslators.Count > 0)
				{
					language = currentBook.TranslatedLanguage;
				}
				else
				{
					var selectedLanguage = ConsoleHelper.ReadLanguage(Messages.SelectTranslationLanguage);
					if (selectedLanguage is null) break;
					language = selectedLanguage.Value;
				}

				return (currentTranslators.Concat(newlySelected).DistinctBy(t => t.Id).ToList(), language);
			}
			case "books.edit.translator.remove-one":
			{
				if (currentBook.Translators.Count == 0)
				{
					ConsoleHelper.ShowWarning(Messages.NoTranslatorToRemove);
					break;
				}

				var selectedForRemoval = ConsoleHelper.ReadTranslatorDtos(Messages.TranslatorSelectionForRemove,
					currentBook.Translators, false, false);
				if (selectedForRemoval is null || selectedForRemoval.Count == 0) break;

				var translatorToRemove = selectedForRemoval[0];
				return ([.. currentBook.Translators.Where(t => t.Id != translatorToRemove.Id)], null);
			}
			case "books.edit.translator.replace":
			{
				var allTranslators = translatorManagementService.GetAllTranslators();
				if (allTranslators.Count == 0)
				{
					ConsoleHelper.ShowWarning(Messages.NotAvailableTranslator);
					break;
				}

				var newTranslators =
					ConsoleHelper.ReadTranslatorDtos(Messages.SelectReplacementTranslators, allTranslators);
				if (newTranslators is null || newTranslators.Count == 0) break;

				var language = ConsoleHelper.ReadLanguage(Messages.SelectTranslationLanguage);
				if (language is null) break;

				return (newTranslators, language);
			}
			case "books.edit.translator.remove-all":
			{
				if (currentBook.Translators.Count == 0)
				{
					ConsoleHelper.ShowWarning("This book already has no translators.");
					break;
				}

				var confirm = ConsoleHelper.ReadYesNo(Messages.RemoveAllTranslators);
				if (confirm != true) break;

				return ([], null);
			}
			case "books.edit.translator.back":
			{
				ConsoleHelper.ShowInfo("Translator edit cancelled. Returning to Edit Menu...");
				break;
			}
		}

		return null;
	}


	private static void RemoveBook(BookManagementService bookManagementService)
	{
		var desiredBook = SelectExistingBook(bookManagementService, "REMOVING BOOK MENU");
		if (desiredBook is null) return;

		BookPrinter.PrintDetails(desiredBook);
		var choice =
			ConsoleHelper.ReadYesNo(string.Format(Messages.BookRemoveConfirmation, desiredBook.Title));

		if (choice != true) return;
		var result = bookManagementService.RemoveBook(desiredBook.Id);
		ConsoleHelper.ShowResult(result);
	}


	private static void SearchBook(BookManagementService bookManagementService, IAuthorizationService authorization,
		IMenuService menuService)
	{
		if (bookManagementService.GetAllBooks().Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.NotAvailableBook);
			return;
		}

		while (true)
		{
			Console.Clear();
			var selectedId = MenuHelper.DisplayMenu("SEARCHING BOOK MENU", menuService.GetChildren("books.search"));
			if (selectedId is null) return;

			if (selectedId.Equals("books.search.back", StringComparison.OrdinalIgnoreCase))
			{
				ConsoleHelper.ShowInfo(string.Format(Messages.SearchCancelled, "Book"));
				return;
			}

			var searchMenuChoice = ConsoleHelper.ReadInt(Messages.SearchMenuQuestion, 1, 8);
			if (searchMenuChoice == null) return;

			switch (selectedId.ToLowerInvariant())
			{
				case "books.search.title":
				{
					Console.Clear();
					var searchTerm = ConsoleHelper.ReadString("Enter a title to search");
					if (string.IsNullOrWhiteSpace(searchTerm)) continue;
					var results = bookManagementService.SearchBooks(searchTerm, BookSearchField.BookName);
					DisplayBookResults(results, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case "books.search.isbn":
				{
					Console.Clear();
					var searchTerm = ConsoleHelper.ReadISBN("Enter an ISBN to search");
					if (string.IsNullOrWhiteSpace(searchTerm)) continue;
					var results = bookManagementService.SearchBooks(searchTerm, BookSearchField.ISBN);
					DisplayBookResults(results, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case "books.search.author":
				{
					Console.Clear();
					var searchTerm = ConsoleHelper.ReadString("Enter an author name");
					if (string.IsNullOrWhiteSpace(searchTerm)) continue;
					var results = bookManagementService.SearchBooks(searchTerm, BookSearchField.AuthorName);
					DisplayBookResults(results, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case "books.search.translator":
				{
					Console.Clear();
					var searchTerm = ConsoleHelper.ReadString("Enter a translator name");
					if (string.IsNullOrWhiteSpace(searchTerm)) continue;
					var results = bookManagementService.SearchBooks(searchTerm, BookSearchField.TranslatorName);
					DisplayBookResults(results, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case "books.search.publishdate":
				{
					Console.Clear();
					var results = SearchByPublishDate(bookManagementService, menuService);
					DisplayBookResults(results!, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case "books.search.genre":
				{
					Console.Clear();
					var searchTerm = ConsoleHelper.ReadGenre("Enter a genre to search");
					if (searchTerm is null) continue;
					var results = bookManagementService.SearchBooks(searchTerm.Value.ToString(), BookSearchField.Genre);
					DisplayBookResults(results, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case "books.search.publisher":
				{
					Console.Clear();
					var searchTerm = ConsoleHelper.ReadString("Enter a publisher to search");
					if (string.IsNullOrWhiteSpace(searchTerm)) continue;
					var results = bookManagementService.SearchBooks(searchTerm, BookSearchField.Publisher);
					DisplayBookResults(results, authorization);
					ConsoleHelper.Pause();
					break;
				}
				default:
				{
					ConsoleHelper.ShowError(Messages.InvalidMenuChoice);
					ConsoleHelper.Pause();
					continue;
				}
			}
		}
	}


	private static void DisplayBookResults(IReadOnlyList<BookDto> result, IAuthorizationService authorization)
	{
		if (result.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.NotBookMatched);
			return;
		}

		BookPrinter.PrintTable(result, authorization, "Search Result");
	}


	private static IReadOnlyList<BookDto>? SearchByPublishDate(BookManagementService bookManagementService,
		IMenuService menuService)
	{
		while (true)
		{
			Console.Clear();

			var selectedId = MenuHelper.DisplayMenu("SEARCH BY PUBLISH DATE",
				menuService.GetChildren("books.search.publish-date"));
			if (selectedId is null) return null;

			if (selectedId.Equals("books.search.publish-date.back", StringComparison.OrdinalIgnoreCase)) return null;

			DateOnly from;
			DateOnly to;
			switch (selectedId.ToLowerInvariant())
			{
				case "books.search.publish-date.exact":
				{
					var date = ConsoleHelper.ReadDateOnly("Enter a publish date to search");
					if (date is null) return null;
					from = to = date.Value;
					break;
				}
				case "books.search.publish-date.before":
				{
					var date = ConsoleHelper.ReadDateOnly("Enter date (find books published before this date)");
					if (date is null) return null;
					from = DateOnly.MinValue;
					to = date.Value;
					break;
				}
				case "books.search.publish-date.after":
				{
					var date = ConsoleHelper.ReadDateOnly("Enter date (find books published after this date)");
					if (date is null) return null;
					from = date.Value;
					to = DateOnly.MaxValue;
					break;
				}
				case "books.search.publish-date.date-range":
				{
					var startDate = ConsoleHelper.ReadDateOnly("Enter start date");
					if (startDate is null) return null;

					var endDate = ConsoleHelper.ReadDateOnly("Enter end date");
					if (endDate is null) return null;

					if (endDate.Value < startDate.Value)
					{
						ConsoleHelper.ShowError("End date cannot be before start date.");
						ConsoleHelper.Pause();
						continue;
					}

					from = startDate.Value;
					to = endDate.Value;
					break;
				}
				default:
					ConsoleHelper.ShowError(Messages.InvalidMenuChoice);
					ConsoleHelper.Pause();
					continue;
			}

			return bookManagementService.SearchBooksByDate(from, to, book => book.PublishDate);
		}
	}



	private static void SortBook(BookManagementService bookManagementService, IAuthorizationService authorization,
		IMenuService menuService)
	{
		MenuHelper.SortByMenu<BookSortField, BookDto>(
			menuService,
			"books.sort",
			"SORT BOOKS",
			TryMapBookSortField,
			bookManagementService.GetAllBooks,
			display: (books, description) =>
				BookPrinter.PrintTable(books, authorization, $"Sorted Books - {description}"),
			Messages.NotAvailableBook,
			"Book",
			"books.sort.back");
	}


	private static bool TryMapBookSortField(string menuId, out BookSortField field)
	{
		switch (menuId.ToLowerInvariant())
		{
			case "books.sort.title":
				field = BookSortField.Title;
				return true;
			case "books.sort.isbn":
				field = BookSortField.ISBN;
				return true;
			case "books.sort.author":
				field = BookSortField.Author;
				return true;
			case "books.sort.translator":
				field = BookSortField.Translator;
				return true;
			case "books.sort.publish-date":
				field = BookSortField.PublishDate;
				return true;
			case "books.sort.genre":
				field = BookSortField.Genre;
				return true;
			case "books.sort.publisher":
				field = BookSortField.Publisher;
				return true;
			case "books.sort.availablecopies":
				field = BookSortField.AvailableCopies;
				return true;
			default:
				field = default;
				return false;
		}
	}


	private static void ViewBooks(BookManagementService bookManagementService,
		LoanManagementService loanManagementService, IAuthorizationService authorization,
		ICurrentUserSession session, LibraryStatisticsService statisticsService, IMenuService menuService)
	{
		while (true)
		{
			ConsoleHelper.ClearConsole();
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);

			switch (ViewBookMenuList(menuService))
			{
				case "books.view.book-details":
				{
					if (!authorization.HasPermission(Permission.ViewBookDetails)) break;
					Console.Clear();
					ViewBookDetails(bookManagementService, loanManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case "books.view.all-books":
				{
					if (!authorization.HasPermission(Permission.ViewAllBooks)) break;
					Console.Clear();
					var books = bookManagementService.GetAllBooks();
					if (books.Count is 0)
						ConsoleHelper.ShowWarning(Messages.NotAvailableBook);
					else
						BookPrinter.PrintTable(books, authorization, "All Books");
					ConsoleHelper.Pause();
					break;
				}
				case "books.view.removed-books":
				{
					if (!authorization.HasPermission(Permission.ViewRemovedBooks)) break;
					Console.Clear();
					var removedBooks = bookManagementService.GetRemovedBooks();
					if (removedBooks.Count is 0)
						ConsoleHelper.ShowWarning(Messages.NotAvailableRemovedBook);
					else
						BookPrinter.PrintTable(removedBooks);
					ConsoleHelper.Pause();
					break;
				}
				case "books.view.back":
				{
					ConsoleHelper.ShowInfo(Messages.BackToBookMenu);
					return;
				}
			}
		}
	}


	private static string? ViewBookMenuList(IMenuService menuService)
	{
		var items = menuService.GetChildren("books.view");
		return MenuHelper.DisplayMenu("VIEW BOOK MENU", items);
	}


	private static void ViewBookDetails(BookManagementService bookManagementService,
		LoanManagementService loanManagementService, ICurrentUserSession session)
	{
		var desiredBook = SelectExistingBook(bookManagementService, "Book Details");
		if (desiredBook is null) return;
		BookPrinter.PrintDetails(desiredBook);

		if (session.IsSelfServiceMember)
		{
			var ownLoans = loanManagementService.GetOwnLoansByBook(desiredBook.Id, session);
			BookPrinter.PrintLoanHistory(ownLoans);
			return;
		}

		var loans = loanManagementService.GetLoanByBook(desiredBook.Id, session, LoanFilter.Active);
		BookPrinter.PrintLoanHistory(loans);
	}
}