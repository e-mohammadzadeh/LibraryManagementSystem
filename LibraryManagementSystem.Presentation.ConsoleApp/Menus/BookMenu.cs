using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Books;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class BookMenu
{
	public static void BookMenuController(AuthorManagementService authorManagementService,
		TranslatorManagementService translatorManagementService, BookManagementService bookManagementService,
		LoanManagementService loanManagementService, LibraryStatisticsService statisticsService,
		ICurrentUserSession session)
	{
		if (!SessionGuard.RequireBookManagement(session)) return;

		var continueProgram = true;
		while (continueProgram)
		{
			if (!session.IsAuthenticated)
			{
				ConsoleHelper.ShowError(ValidationMessages.SessionExpired);
				ConsoleHelper.Pause();
				return;
			}

			Console.Clear();
			MenuHelper.Print(statisticsService.GetLibraryStatistics());
			switch (BookMenuList(session))
			{
				case 1:
				{
					Console.Clear();
					AddBook(authorManagementService, translatorManagementService, bookManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					Console.Clear();
					EditBook(authorManagementService, translatorManagementService, bookManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					Console.Clear();
					RemoveBook(bookManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					Console.Clear();
					SearchBook(bookManagementService);
					break;
				}
				case 5:
				{
					Console.Clear();
					ViewBookDetails(bookManagementService, loanManagementService);
					break;
				}
				case 6:
				{
					Console.Clear();
					if (bookManagementService.GetAllBooks().Count is 0)
						ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableBook);
					else
						BookPrinter.PrintTable(bookManagementService.GetAllBooks());

					ConsoleHelper.Pause();
					break;
				}
				case 7:
				{
					ConsoleHelper.ShowInfo(ValidationMessages.BackToMainMenu);
					ConsoleHelper.Pause();
					Console.Clear();
					continueProgram = false;
					break;
				}
			}
		}
	}


	private static int BookMenuList(ICurrentUserSession session)
	{
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "Add Book", session.IsAdmin || session.IsLibrarian),
			(2, "Edit Book", session.IsAdmin || session.IsLibrarian),
			(3, "Remove Book", session.IsAdmin || session.IsLibrarian),
			(4, "Search Book", true),
			(5, "View Book Details", true),
			(6, "View All Books", true),
			(7, "Back", true)
		};

		var availableItems = items.Where(i => i.IsAvailable).ToList();

		while (true)
		{
			Console.WriteLine(new string('=', 36) + " BOOK MENU " + new string('=', 36));

			var displayNumber = 1;
			foreach (var item in availableItems)
			{
				Console.WriteLine($"{displayNumber}. {item.DisplayText}");
				displayNumber++;
			}

			Console.WriteLine(new string('=', 82));
			Console.Write(ValidationMessages.MainMenuQuestion);

			var option = Console.ReadLine();
			if (!int.TryParse(option, out var userChoice))
			{
				ConsoleHelper.ShowError(ValidationMessages.InvalidMenuChoice);
				continue;
			}

			if (userChoice >= 1 && userChoice <= availableItems.Count) return availableItems[userChoice - 1].ActionId;

			ConsoleHelper.ShowError(ValidationMessages.InvalidMenuChoice);
		}
	}


	private static void AddBook(AuthorManagementService authorManagementService,
		TranslatorManagementService translatorManagementService, BookManagementService bookManagementService,
		ICurrentUserSession session)
	{
		if (session is { IsAdmin: false, IsLibrarian: false })
		{
			ConsoleHelper.ShowError(ValidationMessages.AccessDenied);
			return;
		}

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
		var genreId = ConsoleHelper.ReadInt("Select your desired genre by entering its ID", 1,
			Enum.GetValues<Genre>().Length);
		if (genreId is null) return;

		var publisher = ConsoleHelper.GetValidName("Enter the publisher for this book",
			ValidationConstants.MinPublisherNameLength, ValidationConstants.MaxPublisherNameLength);
		if (publisher is null) return;

		var description = ConsoleHelper.ReadString("You can add any descriptions about this book (Optional)", true);

		var result = bookManagementService.AddBook(new CreateBookDto
		{
			ISBN = isbn,
			BookName = bookName,
			AuthorIds = authorIds,
			TranslatorIds = translatorIds,
			PublishDate = publishDate.Value,
			TotalCopies = totalCopies.Value,
			GenreId = genreId.Value - 1,
			Publisher = publisher,
			Description = description
		});

		ConsoleHelper.ShowResult(result);
	}


	private static List<int>? ResolveAuthorIds(AuthorManagementService authorManagementService)
	{
		var availableAuthors = authorManagementService.GetAllAuthors();
		if (availableAuthors.Count != 0)
			return ConsoleHelper.ReadAuthors("Select author(s) for this book", availableAuthors);

		var choice = ConsoleHelper.ReadYesNo(ValidationMessages.NotAvailableAuthor);
		if (choice is not true)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.BookRequiresAtLeastOneAuthor);
			return null;
		}

		var authorDto = AuthorMenu.PromptForAuthorDto();
		if (authorDto is null) return null;

		var addResult = authorManagementService.AddAuthor(authorDto);
		ConsoleHelper.ShowResult(addResult);
		if (addResult is { Success: true, Data: not null }) return [addResult.Data.Id];

		ConsoleHelper.ShowError(ValidationMessages.AuthorCreationFailed);
		return null;
	}


	private static List<int>? ResolveTranslatorIds(TranslatorManagementService translatorManagementService)
	{
		var availableTranslators = translatorManagementService.GetAllTranslators();
		if (availableTranslators.Count != 0)
			return ConsoleHelper.ReadTranslators("Select one or more translators (optional)", availableTranslators);

		var choice = ConsoleHelper.ReadYesNo(ValidationMessages.AddTranslatorInAdd);
		if (choice != true) return [];

		var translatorDto = TranslatorMenu.PromptForTranslatorDto();
		if (translatorDto is null) return null;

		var addResult = translatorManagementService.AddTranslator(translatorDto);
		ConsoleHelper.ShowResult(addResult);
		if (addResult is { Success: true, Data: not null }) return [addResult.Data.Id];

		ConsoleHelper.ShowError(ValidationMessages.NotAvailableTranslator);
		return null;
	}



	private static void EditBook(AuthorManagementService authorManagementService,
		TranslatorManagementService translatorManagementService, BookManagementService bookManagementService,
		ICurrentUserSession session)
	{
		if (session is { IsAdmin: false, IsLibrarian: false })
		{
			ConsoleHelper.ShowError(ValidationMessages.AccessDenied);
			return;
		}

		Console.WriteLine(new string('=', 36) + " EDITING BOOK MENU " + new string('=', 36));
		var desiredBook = SelectExistingBook(bookManagementService);
		if (desiredBook is null) return;

		while (true)
		{
			var authorNameDisplay = string.Join(", ", desiredBook.Authors.Select(a => a.FullName));
			var translatorNameDisplay = string.Join(", ", desiredBook.Translators.Select(t => t.FullName));
			Console.WriteLine("\n{0, -30} [{1}]", "1. Book Name", desiredBook.BookName);
			Console.WriteLine("{0, -30} [{1}]", "2. ISBN", desiredBook.ISBN);
			Console.WriteLine("{0, -30} [{1}]", "3. Author(s)", authorNameDisplay);
			Console.WriteLine("{0, -30} [{1}]", "4. Translator(s)",
				string.IsNullOrWhiteSpace(translatorNameDisplay) ? "None" : translatorNameDisplay);
			Console.WriteLine("{0, -30} [{1}]", "5. Publish Date", desiredBook.PublishDate);
			Console.WriteLine("{0, -30} [{1}]", "6. Total Copies", desiredBook.TotalCopies);
			Console.WriteLine("{0, -30} [{1}]", "7. Genre", desiredBook.Genre);
			Console.WriteLine("{0, -30} [{1}]", "8. Publisher", desiredBook.Publisher);
			Console.WriteLine("{0, -30} [{1}]", "9. Description", desiredBook.Description);
			Console.WriteLine("10. Cancel");
			var editMenuChoice = ConsoleHelper.ReadInt(ValidationMessages.EditMenuQuestion, 1, 10);
			if (editMenuChoice is null) return;

			switch (editMenuChoice)
			{
				case 1:
				{
					var bookName = ConsoleHelper.GetValidName("Enter the new book name",
						ValidationConstants.MinBookNameLength, ValidationConstants.MaxBookNameLength);

					PerformUpdate(bookManagementService, desiredBook.BookId, bookName,
						v => new UpdateBookDto { BookName = v });

					break;
				}
				case 2:
				{
					var isbn = ConsoleHelper.ReadISBN("Enter the new ISBN");
					PerformUpdate(bookManagementService, desiredBook.BookId, isbn,
						v => new UpdateBookDto { ISBN = v });

					break;
				}
				case 3:
				{
					AuthorSubMenu(desiredBook.BookId, authorManagementService, bookManagementService);
					break;
				}
				case 4:
				{
					TranslatorSubMenu(desiredBook.BookId, translatorManagementService, bookManagementService);
					break;
				}
				case 5:
				{
					var publishDate = ConsoleHelper.GetValidDate("Enter the new publish date");
					PerformUpdate(bookManagementService, desiredBook.BookId, publishDate,
						v => new UpdateBookDto { PublishDate = v });

					break;
				}
				case 6:
				{
					var totalCopies = ConsoleHelper.ReadInt("Enter the new total copies",
						ValidationConstants.MinBookCopies, ValidationConstants.MaxBookCopies);

					PerformUpdate(bookManagementService, desiredBook.BookId, totalCopies,
						v => new UpdateBookDto { TotalCopies = v });

					break;
				}
				case 7:
				{
					ConsoleHelper.DisplayGenres();
					var genreId =
						ConsoleHelper.ReadInt("Enter the new genre id", 1, Enum.GetValues<Genre>().Length + 1);

					if (genreId is null) break;

					PerformUpdate(bookManagementService, desiredBook.BookId, genreId - 1,
						v => new UpdateBookDto { GenreId = v });

					break;
				}
				case 8:
				{
					var publisher = ConsoleHelper.GetValidName("Enter the new publisher",
						ValidationConstants.MinPublisherNameLength, ValidationConstants.MaxPublisherNameLength);
					PerformUpdate(bookManagementService, desiredBook.BookId, publisher,
						v => new UpdateBookDto { Publisher = v });
					break;
				}
				case 9:
				{
					var description = ConsoleHelper.ReadString("Enter the new description");
					PerformUpdate(bookManagementService, desiredBook.BookId, description,
						v => new UpdateBookDto { Description = v });

					break;
				}
				case 10:
				{
					ConsoleHelper.ShowError(string.Format(ValidationMessages.EditCancelled, "Book"));
					ConsoleHelper.Pause();
					Console.Clear();
					return;
				}
			}

			var choice = ConsoleHelper.ReadYesNo(ValidationMessages.EditContinuesQuestion);
			if (choice != true)
			{
				Console.Clear();
				return;
			}

			// Refresh desiredBook details for subsequent edits in loop
			var refreshedBook = bookManagementService.FindBookById(desiredBook.BookId);
			if (refreshedBook is not null) desiredBook = refreshedBook;

			Console.Clear();
		}
	}


	private static BookDto? SelectExistingBook(BookManagementService bookManagementService)
	{
		var bookList = bookManagementService.GetAllBooks();
		if (bookList.Count is not 0) return MenuHelper.SelectBook(bookList);

		ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableBook);
		return null;
	}


	private static void PerformUpdate<T>(BookManagementService bookManagementService, int desiredBookId,
		T? newValue, Func<T, UpdateBookDto> buildDto)
	{
		if (newValue is null) return;

		var dto = buildDto(newValue);
		var result = bookManagementService.UpdateBook(desiredBookId, dto);
		ConsoleHelper.ShowResult(result);
	}


	private static void AuthorSubMenu(int bookId, AuthorManagementService authorManagementService,
		BookManagementService bookManagementService)
	{
		var currentBook = bookManagementService.FindBookById(bookId);
		if (currentBook is null)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableBook);
			return;
		}

		var currentAuthorIds = currentBook.Authors.Select(a => a.Id).ToHashSet();
		var currentAuthorsNames = string.Join(", ", currentBook.Authors.Select(a => a.FullName));

		Console.WriteLine($"Current authors: {currentAuthorsNames}");
		Console.WriteLine(ValidationMessages.SubMenusQuestion);
		Console.WriteLine("1. Add an author");
		Console.WriteLine("2. Remove an author");
		Console.WriteLine("3. Replace all authors");
		Console.WriteLine("4. Cancel");
		var editMenuChoice = ConsoleHelper.ReadInt(ValidationMessages.EditMenuQuestion, 1, 4);
		if (editMenuChoice is null) return;

		switch (editMenuChoice)
		{
			case 1:
			{
				var allAuthors = authorManagementService.GetAllAuthors();
				if (allAuthors.Count == 0)
				{
					ConsoleHelper.ShowError(ValidationMessages.NotAvailableAuthor);
					break;
				}

				var availableToAdd = allAuthors.Where(a => !currentAuthorIds.Contains(a.Id)).ToList().AsReadOnly();
				if (availableToAdd.Count == 0)
				{
					ConsoleHelper.ShowError(ValidationMessages.NotEnoughAuthors);
					break;
				}

				var selectedIds = ConsoleHelper.ReadAuthors("Select author(s) for this book", availableToAdd);
				if (selectedIds is null) return;

				// New list = existing author IDs + newly selected IDs
				var updatedAuthorIds = currentAuthorIds.Concat(selectedIds).Distinct().ToList();
				PerformUpdate(bookManagementService, bookId, updatedAuthorIds,
					v => new UpdateBookDto { AuthorIds = v });
				break;
			}
			case 2:
			{
				if (currentBook.Authors.Count <= 1)
				{
					ConsoleHelper.ShowWarning(ValidationMessages.CannotRemove);
					break;
				}

				// Use ReadAuthors with allowMultiple = false — pick exactly one to remove
				var selectedIds = ConsoleHelper.ReadAuthors(ValidationMessages.AuthorSelection4Remove,
					currentBook.Authors,
					false);
				if (selectedIds is null) break;

				var idToRemove = selectedIds[0];
				var updatedAuthorIds = currentBook.Authors.Select(a => a.Id).Where(id => id != idToRemove).ToList();

				PerformUpdate(bookManagementService, bookId, updatedAuthorIds,
					v => new UpdateBookDto { AuthorIds = v });
				break;
			}
			case 3:
			{
				var allAuthors = authorManagementService.GetAllAuthors();
				if (allAuthors.Count == 0)
				{
					ConsoleHelper.ShowError(ValidationMessages.NotAvailableActionLoan);
					break;
				}

				var selectedIds = ConsoleHelper.ReadAuthors("Select the new author(s) for this book", allAuthors);
				if (selectedIds is null) break;

				PerformUpdate(bookManagementService, bookId, selectedIds,
					v => new UpdateBookDto { AuthorIds = v });
				break;
			}
			case 4:
			{
				ConsoleHelper.ShowError("Author edit cancelled. Returning to Edit Menu...");
				ConsoleHelper.Pause();
				Console.Clear();
				break;
			}
		}
	}


	private static void TranslatorSubMenu(int bookId, TranslatorManagementService translatorManagementService,
		BookManagementService bookManagementService)
	{
		var currentBook = bookManagementService.FindBookById(bookId);
		if (currentBook is null)
		{
			ConsoleHelper.ShowError(ValidationMessages.NotAvailableBook);
			return;
		}

		var currentTranslatorName = currentBook.Translators.Count == 0
			? "None"
			: string.Join(", ", currentBook.Translators.Select(t => t.FullName));

		Console.WriteLine($"Current translators: {currentTranslatorName}");
		Console.WriteLine(ValidationMessages.SubMenusQuestion);
		Console.WriteLine("1. Add a translator");
		Console.WriteLine("2. Remove a translator");
		Console.WriteLine("3. Replace all translators");
		Console.WriteLine("4. Remove all translators");
		Console.WriteLine("5. Cancel");
		var editMenuChoice = ConsoleHelper.ReadInt(ValidationMessages.EditMenuQuestion, 1, 5);
		if (editMenuChoice is null) return;

		switch (editMenuChoice)
		{
			case 1:
			{
				var allTranslators = translatorManagementService.GetAllTranslators();
				if (allTranslators.Count == 0)
				{
					ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableTranslator);
					break;
				}

				var currentTranslatorIds = currentBook.Translators.Select(t => t.Id).ToHashSet();
				var availableToAdd = allTranslators.Where(t => !currentTranslatorIds.Contains(t.Id)).ToList()
					.AsReadOnly();
				if (availableToAdd.Count == 0)
				{
					ConsoleHelper.ShowWarning(ValidationMessages.AllTranslatorsAssigned);
					break;
				}

				var selectedIds = ConsoleHelper.ReadTranslators("Select translator(s) to add", availableToAdd);
				if (selectedIds is null || selectedIds.Count == 0) break;

				var updatedTranslatorIds = currentTranslatorIds.Concat(selectedIds).Distinct().ToList();
				PerformUpdate(bookManagementService, bookId, updatedTranslatorIds,
					v => new UpdateBookDto { TranslatorIds = v });
				break;
			}
			case 2:
			{
				if (currentBook.Translators.Count == 0)
				{
					ConsoleHelper.ShowWarning(ValidationMessages.NoTranslatorToRemove);
					break;
				}

				var selectedIds = ConsoleHelper.ReadTranslators(ValidationMessages.TranslatorSelection4Remove,
					currentBook.Translators, false, false);
				if (selectedIds is null) break;
				var idToRemove = selectedIds[0];
				var updatedTranslatorIds = currentBook.Translators.Select(t => t.Id)
					.Where(id => id != idToRemove).ToList();

				PerformUpdate(bookManagementService, bookId, updatedTranslatorIds,
					v => new UpdateBookDto { TranslatorIds = v });
				break;
			}
			case 3:
			{
				var allTranslators = translatorManagementService.GetAllTranslators();
				if (allTranslators.Count == 0)
				{
					ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableTranslator);
					break;
				}

				var selectedIds = ConsoleHelper.ReadTranslators(ValidationMessages.AddTranslatorInEdit, allTranslators);
				if (selectedIds is null) break;
				PerformUpdate(bookManagementService, bookId, selectedIds,
					v => new UpdateBookDto { TranslatorIds = v });
				break;
			}
			case 4:
			{
				if (currentBook.Translators.Count == 0)
				{
					ConsoleHelper.ShowWarning("This book already has no translators.");
					break;
				}

				var confirm = ConsoleHelper.ReadYesNo(ValidationMessages.RemoveAllTranslators);
				if (confirm != true) break;

				var emptyList = new List<int>();
				PerformUpdate(bookManagementService, bookId, emptyList,
					v => new UpdateBookDto { TranslatorIds = v });
				break;
			}
			case 5:
			{
				ConsoleHelper.ShowError("Translator edit cancelled. Returning to Edit Menu...");
				ConsoleHelper.Pause();
				Console.Clear();
				break;
			}
		}
	}


	private static void RemoveBook(BookManagementService bookManagementService, ICurrentUserSession session)
	{
		if (session is { IsAdmin: false, IsLibrarian: false })
		{
			ConsoleHelper.ShowError(ValidationMessages.AccessDenied);
			return;
		}

		Console.WriteLine(new string('=', 36) + " REMOVING BOOK MENU " + new string('=', 36));
		var desiredBook = SelectExistingBook(bookManagementService);
		if (desiredBook is null) return;

		BookPrinter.PrintDetails(desiredBook);
		var choice =
			ConsoleHelper.ReadYesNo(string.Format(ValidationMessages.BookRemoveConfirmation, desiredBook.BookName));

		if (choice != true) return;
		var result = bookManagementService.RemoveBook(desiredBook.BookId);
		ConsoleHelper.ShowResult(result);
	}


	private static void SearchBook(BookManagementService bookManagementService)
	{
		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 36) + " SEARCHING BOOK MENU " + new string('=', 36));
			var booksList = bookManagementService.GetAllBooks();
			if (booksList.Count == 0)
			{
				ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableBook);
				ConsoleHelper.Pause();
				return;
			}

			Console.WriteLine("\n{0, -20}", "1. Title");
			Console.WriteLine("{0, -20}", "2. ISBN");
			Console.WriteLine("{0, -20}", "3. Author");
			Console.WriteLine("{0, -20}", "4. Translator");
			Console.WriteLine("{0, -20}", "5. Publish Date");
			Console.WriteLine("{0, -20}", "6. Genre");
			Console.WriteLine("{0, -20}", "7. Publisher");
			Console.WriteLine("8. Cancel");

			var searchMenuChoice = ConsoleHelper.ReadInt(ValidationMessages.SearchMenuQuestion, 1, 8);
			if (searchMenuChoice == null) return;

			switch (searchMenuChoice)
			{
				case 1:
				{
					SearchBookAndDisplay(bookManagementService, p => ConsoleHelper.ReadString(p),
						"Enter a title to search",
						book => book.BookName, ContainsComparer);

					break;
				}
				case 2:
				{
					SearchBookAndDisplay(bookManagementService, ConsoleHelper.ReadISBN, "Enter an ISBN to search",
						book => book.InternationalStandardBookNumber, ContainsComparer);

					break;
				}
				case 3:
				{
					SearchBookAndDisplay(bookManagementService, p => ConsoleHelper.ReadString(p),
						"Enter an author name",
						book => string.Join(" ",
							book.BookAuthors.Select(ba => $"{ba.Author.FirstName} {ba.Author.LastName}")),
						ContainsComparer);
					break;
				}
				case 4:
				{
					SearchBookAndDisplay(bookManagementService, p => ConsoleHelper.ReadString(p),
						"Enter a translator name",
						book => book.BookTranslators.Count == 0
							? null
							: string.Join(" ",
								book.BookTranslators.Select(bt =>
									$"{bt.Translator.FirstName} {bt.Translator.LastName}")),
						ContainsComparer);
					break;
				}
				case 5:
				{
					SearchBookAndDisplay(bookManagementService, ConsoleHelper.ReadDateOnly,
						"Enter a publish date to search", book => book.PublishDate, DateComparer);

					break;
				}
				case 6:
				{
					SearchBookAndDisplay(bookManagementService, ConsoleHelper.ReadGenre, "Enter a genre to search",
						book => book.Genre, GenreComparer);

					break;
				}
				case 7:
				{
					SearchBookAndDisplay(bookManagementService, p => ConsoleHelper.ReadString(p),
						"Enter a publisher to search", book => book.Publisher, ContainsComparer);
					break;
				}
				case 8:
				{
					ConsoleHelper.ShowInfo(string.Format(ValidationMessages.SearchCancelled, "Book"));
					ConsoleHelper.Pause();
					Console.Clear();
					return;
				}
			}

			ConsoleHelper.Pause();
		}
	}


	private static void SearchBookAndDisplay<T>(BookManagementService bookManagementService, Func<string, T?> reader,
		string prompt, Func<Book, T?> selector, Func<T, T, bool> comparer) where T : class
	{
		var searchTerm = reader(prompt);
		if (searchTerm is null) return;

		var result = bookManagementService.SearchBooks(searchTerm, selector, comparer);
		DisplayBookResults(result);
	}


	private static void SearchBookAndDisplay<T>(BookManagementService bookManagementService, Func<string, T?> reader,
		string prompt, Func<Book, T?> selector, Func<T, T, bool> comparer) where T : struct
	{
		var searchTerm = reader(prompt);
		if (!searchTerm.HasValue) return;

		var result = bookManagementService.SearchBooks(searchTerm, selector, comparer);
		DisplayBookResults(result);
	}


	private static readonly Func<string, string, bool> ContainsComparer =
		(search, value) => value.Contains(search, StringComparison.OrdinalIgnoreCase);

	private static readonly Func<DateOnly, DateOnly, bool> DateComparer = (search, value) => search == value;
	private static readonly Func<Genre, Genre, bool> GenreComparer = (search, value) => search == value;


	private static void DisplayBookResults(IReadOnlyList<BookDto> result)
	{
		if (result.Count == 0)
		{
			ConsoleHelper.ShowWarning(ValidationMessages.NotBookMatched);
			return;
		}

		BookPrinter.PrintTable(result);
	}


	private static void ViewBookDetails(BookManagementService bookManagementService,
		LoanManagementService loanManagementService)
	{
		var desiredBook = SelectExistingBook(bookManagementService);
		if (desiredBook is not null)
		{
			BookPrinter.PrintDetails(desiredBook);
			var loans = loanManagementService.GetLoanByBook(desiredBook.BookId);
			BookPrinter.PrintLoanHistory(loans);
		}

		ConsoleHelper.Pause();
	}
}