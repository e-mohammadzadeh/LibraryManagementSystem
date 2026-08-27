using System.Text;
using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.Authorization;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.Authors;
using LibraryManagementSystem.Application.DTOs.Books;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Enums.Search;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;
using LibraryManagementSystem.Presentation.ConsoleApp.Printers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public static class BookMenu
{
	public static void BookMenuController(AuthorManagementService authorManagementService,
		TranslatorManagementService translatorManagementService, BookManagementService bookManagementService,
		LoanManagementService loanManagementService, LibraryStatisticsService statisticsService,
		ICurrentUserSession session, IAuthorizationService authorization)
	{
		if (!SessionGuard.RequireAnyPermission(
			    authorization,
			    Messages.AccessDenied,
			    Permission.AddBook,
			    Permission.EditBook,
			    Permission.RemoveBook,
			    Permission.SearchBook,
			    Permission.ViewBookDetails,
			    Permission.ViewAllBooks))
		{
			return;
		}

		var continueProgram = true;
		while (continueProgram)
		{
			if (!session.IsAuthenticated)
			{
				ConsoleHelper.ShowError(Messages.SessionExpired);
				ConsoleHelper.Pause();
				return;
			}

			ConsoleHelper.ClearConsole();
			if (authorization.CanAccessStatistics())
				MenuHelper.Print(statisticsService.GetLibraryStatistics(session), session.CurrentUser);
			else
				MenuHelper.PrintCurrentUserOnly(session.CurrentUser);
			switch (BookMenuList(authorization))
			{
				case 1:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.AddBook, Messages.AccessDenied))
						break;
					Console.Clear();
					AddBook(authorManagementService, translatorManagementService, bookManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.EditBook, Messages.AccessDenied))
						break;
					Console.Clear();
					EditBook(authorManagementService, translatorManagementService, bookManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.RemoveBook, Messages.AccessDenied))
						break;
					Console.Clear();
					RemoveBook(bookManagementService);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.SearchBook, Messages.AccessDenied))
						break;
					Console.Clear();
					SearchBook(bookManagementService, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 5:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewBookDetails,
						    Messages.AccessDenied))
						break;
					Console.Clear();
					ViewBookDetails(bookManagementService, loanManagementService, session);
					ConsoleHelper.Pause();
					break;
				}
				case 6:
				{
					if (!SessionGuard.RequirePermission(authorization, Permission.ViewAllBooks, Messages.AccessDenied))
						break;
					Console.Clear();
					if (bookManagementService.GetAllBooks().Count is 0)
						ConsoleHelper.ShowWarning(Messages.NotAvailableBook);
					else
						BookPrinter.PrintTable(bookManagementService.GetAllBooks(), authorization, "All Books");
					ConsoleHelper.Pause();
					break;
				}
				case 7:
				{
					ConsoleHelper.ShowInfo(Messages.BackToMainMenu);
					continueProgram = false;
					break;
				}
			}
		}
	}


	private static int BookMenuList(IAuthorizationService authorization)
	{
		var items = new List<(int ActionId, string DisplayText, bool IsAvailable)>
		{
			(1, "Add Book", authorization.HasPermission(Permission.AddBook)),
			(2, "Edit Book", authorization.HasPermission(Permission.EditBook)),
			(3, "Remove Book", authorization.HasPermission(Permission.RemoveBook)),
			(4, "Search Book", authorization.HasPermission(Permission.SearchBook)),
			(5, "View Book Details", authorization.HasPermission(Permission.ViewBookDetails)),
			(6, "View All Books", authorization.HasPermission(Permission.ViewAllBooks)),
			(7, "Back", true)
		};

		var availableItems = items.Where(i => i.IsAvailable).ToList();

		while (true)
		{
			Console.WriteLine(new string('=', 36) + " BOOK MENU " + new string('=', 36));

			var displayNumber = 1;
			foreach (var (_, displayText, _) in availableItems)
			{
				Console.WriteLine($"{displayNumber}. {displayText}");
				displayNumber++;
			}

			Console.WriteLine(new string('=', 82));
			Console.Write(Messages.MainMenuQuestion);

			var option = Console.ReadLine();
			if (!int.TryParse(option, out var userChoice))
			{
				ConsoleHelper.ShowError(Messages.InvalidMenuChoice);
				continue;
			}

			if (userChoice >= 1 && userChoice <= availableItems.Count) return availableItems[userChoice - 1].ActionId;

			ConsoleHelper.ShowError(Messages.InvalidMenuChoice);
		}
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

		var choice = ConsoleHelper.ReadYesNo(Messages.NotAvailableAuthor);
		if (choice is not true)
		{
			ConsoleHelper.ShowWarning(Messages.BookRequiresAtLeastOneAuthor);
			return null;
		}

		var createdIds = new List<int>();
		while (true)
		{
			var authorDto = AuthorMenu.PromptForAuthorDto();
			if (authorDto is null)
			{
				if (createdIds.Count == 0) return null;
				break;
			}

			var addResult = authorManagementService.AddAuthor(authorDto);
			ConsoleHelper.ShowResult(addResult);
			if (addResult is { Success: true, Data: not null })
				createdIds.Add(addResult.Data.Id);
			else
				ConsoleHelper.ShowError(Messages.AuthorCreationFailed);

			if (createdIds.Count == 0) continue;

			var addMore = ConsoleHelper.ReadYesNo("Add another author for this book");
			if (addMore != true) break;
		}

		return createdIds.Count > 0 ? createdIds : null;
	}


	private static List<int>? ResolveTranslatorIds(TranslatorManagementService translatorManagementService)
	{
		var availableTranslators = translatorManagementService.GetAllTranslators();
		if (availableTranslators.Count != 0)
			return ConsoleHelper.ReadTranslators("Select one or more translators (optional)", availableTranslators);

		var choice = ConsoleHelper.ReadYesNo(Messages.AddTranslatorInAdd);
		if (choice != true) return [];

		var createdIds = new List<int>();
		while (true)
		{
			var translatorDto = TranslatorMenu.PromptForTranslatorDto();
			if (translatorDto is null) break;
			var addResult = translatorManagementService.AddTranslator(translatorDto);
			ConsoleHelper.ShowResult(addResult);

			if (addResult is { Success: true, Data: not null })
				createdIds.Add(addResult.Data.Id);
			else
				ConsoleHelper.ShowError(Messages.NotAvailableTranslator);

			var addMore = ConsoleHelper.ReadYesNo("Add another translator for this book");
			if (addMore != true) break;
		}

		return createdIds;
	}


	private static void EditBook(AuthorManagementService authorManagementService,
		TranslatorManagementService translatorManagementService, BookManagementService bookManagementService)
	{
		var desiredBook = SelectExistingBook(bookManagementService, "EDITING BOOK MENU");
		if (desiredBook is null) return;

		while (true)
		{
			ConsoleHelper.ClearConsole();
			Console.OutputEncoding = Encoding.UTF8;
			var headers = new[] { "#", "Field", "Current Value" };

			var authorLines = desiredBook.Authors.Count > 0
				? desiredBook.Authors.Select(a => a.FullName).ToArray()
				: ["—"];

			var translatorLines = desiredBook.Translators.Count > 0
				? desiredBook.Translators.Select(t => t.FullName).ToArray()
				: ["None"];

			var descriptionLines = string.IsNullOrWhiteSpace(desiredBook.Description)
				? ["—"]
				: ConsoleTable.WrapText(desiredBook.Description, 40);

			var rows = new List<string[][]>
			{
				new string[][] { ["1"], ["Book Name"], [desiredBook.BookName] },
				new string[][] { ["2"], ["ISBN"], [desiredBook.ISBN] },
				new[] { ["3"], ["Author(s)"], authorLines },
				new[] { ["4"], ["Translator(s)"], translatorLines },
				new string[][] { ["5"], ["Publish Date"], [desiredBook.PublishDate.ToString("yyyy-MM-dd")] },
				new string[][] { ["6"], ["Total Copies"], [desiredBook.TotalCopies.ToString()] },
				new string[][] { ["7"], ["Genre"], [desiredBook.Genre] },
				new string[][] { ["8"], ["Publisher"], [desiredBook.Publisher] },
				new[] { ["9"], ["Description"], descriptionLines },
				new string[][] { ["10"], ["Back"], ["—"] }
			};

			ConsoleTable.PrintTable("Edit Book", headers, rows);

			var editMenuChoice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, 10);
			if (editMenuChoice is null) return;

			switch (editMenuChoice)
			{
				case 1:
				{
					var bookName = ConsoleHelper.GetValidName("\nEnter the new book name",
						ValidationConstants.MinBookNameLength, ValidationConstants.MaxBookNameLength);

					var updated = PerformUpdate(bookManagementService, desiredBook.BookId, bookName,
						v => new UpdateBookDto { BookName = v });
					if (updated is not null) desiredBook = updated;
					break;
				}
				case 2:
				{
					var isbn = ConsoleHelper.ReadISBN("\nEnter the new ISBN");
					var updated = PerformUpdate(bookManagementService, desiredBook.BookId, isbn,
						v => new UpdateBookDto { ISBN = v });
					if (updated is not null) desiredBook = updated;
					break;
				}
				case 3:
				{
					var updated = AuthorSubMenu(desiredBook.BookId, authorManagementService, bookManagementService);
					if (updated is not null) desiredBook = updated;
					break;
				}
				case 4:
				{
					var updated = TranslatorSubMenu(desiredBook.BookId, translatorManagementService,
						bookManagementService);
					if (updated is not null) desiredBook = updated;
					break;
				}
				case 5:
				{
					var publishDate = ConsoleHelper.GetValidDate("\nEnter the new publish date");
					var updated = PerformUpdate(bookManagementService, desiredBook.BookId, publishDate,
						v => new UpdateBookDto { PublishDate = v });
					if (updated is not null) desiredBook = updated;
					break;
				}
				case 6:
				{
					var totalCopies = ConsoleHelper.ReadInt("\nEnter the new total copies",
						ValidationConstants.MinBookCopies, ValidationConstants.MaxBookCopies);

					var updated = PerformUpdate(bookManagementService, desiredBook.BookId, totalCopies,
						v => new UpdateBookDto { TotalCopies = v });
					if (updated is not null) desiredBook = updated;
					break;
				}
				case 7:
				{
					ConsoleHelper.DisplayGenres();
					var genreId =
						ConsoleHelper.ReadInt("\nEnter the new genre id", 1, Enum.GetValues<Genre>().Length + 1);

					if (genreId is null) break;

					var updated = PerformUpdate(bookManagementService, desiredBook.BookId, genreId - 1,
						v => new UpdateBookDto { GenreId = v });
					if (updated is not null) desiredBook = updated;
					break;
				}
				case 8:
				{
					var publisher = ConsoleHelper.GetValidName("\nEnter the new publisher",
						ValidationConstants.MinPublisherNameLength, ValidationConstants.MaxPublisherNameLength);
					var updated = PerformUpdate(bookManagementService, desiredBook.BookId, publisher,
						v => new UpdateBookDto { Publisher = v });
					if (updated is not null) desiredBook = updated;
					break;
				}
				case 9:
				{
					var description = ConsoleHelper.ReadString("\nEnter the new description");
					var updated = PerformUpdate(bookManagementService, desiredBook.BookId, description,
						v => new UpdateBookDto { Description = v });
					if (updated is not null) desiredBook = updated;
					break;
				}
				case 10:
				{
					ConsoleHelper.ShowInfo(string.Format(Messages.EditCancelled, "Book"));
					return;
				}
			}

			var choice = ConsoleHelper.ReadYesNo(Messages.EditContinuesQuestion);
			if (choice != true) return;

			// Refresh desiredBook details for subsequent edits in loop
			var refreshedBook = bookManagementService.FindBookById(desiredBook.BookId);
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


	private static BookDto? PerformUpdate<T>(BookManagementService bookManagementService, int desiredBookId,
		T? newValue, Func<T, UpdateBookDto> buildDto)
	{
		if (newValue is null) return null;

		var dto = buildDto(newValue);
		var result = bookManagementService.UpdateBook(desiredBookId, dto);
		ConsoleHelper.ShowResult(result);
		return result.Data;
	}


	private static BookDto? AuthorSubMenu(int bookId, AuthorManagementService authorManagementService,
		BookManagementService bookManagementService)
	{
		var currentBook = bookManagementService.FindBookById(bookId);
		if (currentBook is null)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableBook);
			return null;
		}

		var currentAuthorIds = currentBook.Authors.Select(a => a.Id).ToHashSet();

		Console.WriteLine(Messages.SubMenuPrompt);
		Console.WriteLine("\t1. Add an author");
		Console.WriteLine("\t2. Remove an author");
		Console.WriteLine("\t3. Replace all authors");
		Console.WriteLine("\t4. Back");
		var editMenuChoice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, 4);
		if (editMenuChoice is null) return null;

		switch (editMenuChoice)
		{
			case 1:
			{
				var allAuthors = authorManagementService.GetAllAuthors();
				if (allAuthors.Count == 0)
				{
					ConsoleHelper.ShowError(Messages.NotAvailableAuthor);
					break;
				}

				IReadOnlyList<AuthorDto> availableToAdd = [.. allAuthors.Where(a => !currentAuthorIds.Contains(a.Id))];
				if (availableToAdd.Count == 0)
				{
					ConsoleHelper.ShowError(Messages.NotEnoughAuthors);
					break;
				}

				var selectedIds = ConsoleHelper.ReadAuthors("\nSelect author(s) for this book", availableToAdd);
				if (selectedIds is null) return null;

				// New list = existing author IDs + newly selected IDs
				var updatedAuthorIds = currentAuthorIds.Concat(selectedIds).Distinct().ToList();
				return PerformUpdate(bookManagementService, bookId, updatedAuthorIds,
					v => new UpdateBookDto { AuthorIds = v });
			}
			case 2:
			{
				if (currentBook.Authors.Count <= 1)
				{
					ConsoleHelper.ShowWarning(Messages.CannotRemoveLastAuthor);
					break;
				}

				var selectedIds = ConsoleHelper.ReadAuthors(Messages.AuthorSelectionForRemove,
					currentBook.Authors,
					false);
				if (selectedIds is null) break;

				var idToRemove = selectedIds[0];
				var updatedAuthorIds = currentBook.Authors.Select(a => a.Id).Where(id => id != idToRemove).ToList();

				return PerformUpdate(bookManagementService, bookId, updatedAuthorIds,
					v => new UpdateBookDto { AuthorIds = v });
			}
			case 3:
			{
				var allAuthors = authorManagementService.GetAllAuthors();
				if (allAuthors.Count == 0)
				{
					ConsoleHelper.ShowError(Messages.NotAvailableActionLoan);
					break;
				}

				var selectedIds = ConsoleHelper.ReadAuthors("\nSelect the new author(s) for this book", allAuthors);
				if (selectedIds is null) break;

				return PerformUpdate(bookManagementService, bookId, selectedIds,
					v => new UpdateBookDto { AuthorIds = v });
			}
			case 4:
			{
				ConsoleHelper.ShowInfo("Author edit cancelled. Returning to Edit Menu...");
				break;
			}
		}

		return null;
	}


	private static BookDto? TranslatorSubMenu(int bookId, TranslatorManagementService translatorManagementService,
		BookManagementService bookManagementService)
	{
		var currentBook = bookManagementService.FindBookById(bookId);
		if (currentBook is null)
		{
			ConsoleHelper.ShowError(Messages.NotAvailableBook);
			return null;
		}

		Console.WriteLine(Messages.SubMenuPrompt);
		Console.WriteLine("\t1. Add a translator");
		Console.WriteLine("\t2. Remove a translator");
		Console.WriteLine("\t3. Replace all translators");
		Console.WriteLine("\t4. Remove all translators");
		Console.WriteLine("\t5. Back");
		var editMenuChoice = ConsoleHelper.ReadInt(Messages.EditMenuQuestion, 1, 5);
		if (editMenuChoice is null) return null;

		switch (editMenuChoice)
		{
			case 1:
			{
				var allTranslators = translatorManagementService.GetAllTranslators();
				if (allTranslators.Count == 0)
				{
					ConsoleHelper.ShowWarning(Messages.NotAvailableTranslator);
					break;
				}

				var currentTranslatorIds = currentBook.Translators.Select(t => t.Id).ToHashSet();
				var availableToAdd = allTranslators.Where(t => !currentTranslatorIds.Contains(t.Id)).ToList()
					.AsReadOnly();
				if (availableToAdd.Count == 0)
				{
					ConsoleHelper.ShowWarning(Messages.AllTranslatorsAssigned);
					break;
				}

				var selectedIds = ConsoleHelper.ReadTranslators("\nSelect translator(s) to add", availableToAdd);
				if (selectedIds is null || selectedIds.Count == 0) break;

				var updatedTranslatorIds = currentTranslatorIds.Concat(selectedIds).Distinct().ToList();
				return PerformUpdate(bookManagementService, bookId, updatedTranslatorIds,
					v => new UpdateBookDto { TranslatorIds = v });
			}
			case 2:
			{
				if (currentBook.Translators.Count == 0)
				{
					ConsoleHelper.ShowWarning(Messages.NoTranslatorToRemove);
					break;
				}

				var selectedIds = ConsoleHelper.ReadTranslators(Messages.TranslatorSelectionForRemove,
					currentBook.Translators, false, false);
				if (selectedIds is null) break;
				var idToRemove = selectedIds[0];
				var updatedTranslatorIds = currentBook.Translators.Select(t => t.Id)
					.Where(id => id != idToRemove).ToList();

				return PerformUpdate(bookManagementService, bookId, updatedTranslatorIds,
					v => new UpdateBookDto { TranslatorIds = v });
			}
			case 3:
			{
				var allTranslators = translatorManagementService.GetAllTranslators();
				if (allTranslators.Count == 0)
				{
					ConsoleHelper.ShowWarning(Messages.NotAvailableTranslator);
					break;
				}

				var selectedIds = ConsoleHelper.ReadTranslators(Messages.SelectReplacementTranslators, allTranslators);
				if (selectedIds is null) break;
				return PerformUpdate(bookManagementService, bookId, selectedIds,
					v => new UpdateBookDto { TranslatorIds = v });
			}
			case 4:
			{
				if (currentBook.Translators.Count == 0)
				{
					ConsoleHelper.ShowWarning("This book already has no translators.");
					break;
				}

				var confirm = ConsoleHelper.ReadYesNo(Messages.RemoveAllTranslators);
				if (confirm != true) break;

				var emptyList = new List<int>();
				return PerformUpdate(bookManagementService, bookId, emptyList,
					v => new UpdateBookDto { TranslatorIds = v });
			}
			case 5:
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
			ConsoleHelper.ReadYesNo(string.Format(Messages.BookRemoveConfirmation, desiredBook.BookName));

		if (choice != true) return;
		var result = bookManagementService.RemoveBook(desiredBook.BookId);
		ConsoleHelper.ShowResult(result);
	}


	private static void SearchBook(BookManagementService bookManagementService, IAuthorizationService authorization)
	{
		while (true)
		{
			Console.Clear();
			Console.WriteLine(new string('=', 36) + " SEARCHING BOOK MENU " + new string('=', 36));
			var booksList = bookManagementService.GetAllBooks();
			if (booksList.Count == 0)
			{
				ConsoleHelper.ShowWarning(Messages.NotAvailableBook);
				return;
			}

			Console.WriteLine("{0, -20}", "1. Title");
			Console.WriteLine("{0, -20}", "2. ISBN");
			Console.WriteLine("{0, -20}", "3. Author");
			Console.WriteLine("{0, -20}", "4. Translator");
			Console.WriteLine("{0, -20}", "5. Publish Date");
			Console.WriteLine("{0, -20}", "6. Genre");
			Console.WriteLine("{0, -20}", "7. Publisher");
			Console.WriteLine("8. Back");

			var searchMenuChoice = ConsoleHelper.ReadInt(Messages.SearchMenuQuestion, 1, 8);
			if (searchMenuChoice == null) return;

			switch (searchMenuChoice)
			{
				case 1:
				{
					Console.Clear();
					var searchTerm = ConsoleHelper.ReadString("Enter a title to search");
					if (string.IsNullOrWhiteSpace(searchTerm)) continue;
					var results = bookManagementService.SearchBooks(searchTerm, BookSearchField.BookName);
					DisplayBookResults(results, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 2:
				{
					Console.Clear();
					var searchTerm = ConsoleHelper.ReadISBN("Enter an ISBN to search");
					if (string.IsNullOrWhiteSpace(searchTerm)) continue;
					var results = bookManagementService.SearchBooks(searchTerm, BookSearchField.ISBN);
					DisplayBookResults(results, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 3:
				{
					Console.Clear();
					var searchTerm = ConsoleHelper.ReadString("Enter an author name");
					if (string.IsNullOrWhiteSpace(searchTerm)) continue;
					var results = bookManagementService.SearchBooks(searchTerm, BookSearchField.AuthorName);
					DisplayBookResults(results, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 4:
				{
					Console.Clear();
					var searchTerm = ConsoleHelper.ReadString("Enter a translator name");
					if (string.IsNullOrWhiteSpace(searchTerm)) continue;
					var results = bookManagementService.SearchBooks(searchTerm, BookSearchField.TranslatorName);
					DisplayBookResults(results, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 5:
				{
					Console.Clear();
					var searchTerm = ConsoleHelper.ReadDateOnly("Enter a publish date to search");
					if (searchTerm is null) continue;
					var results = bookManagementService.SearchBooks(searchTerm.Value.ToString("yyyy-MM-dd"),
						BookSearchField.PublishDate);
					DisplayBookResults(results, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 6:
				{
					Console.Clear();
					var searchTerm = ConsoleHelper.ReadGenre("Enter a genre to search");
					if (searchTerm is null) continue;
					var results = bookManagementService.SearchBooks(searchTerm.Value.ToString(), BookSearchField.Genre);
					DisplayBookResults(results, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 7:
				{
					Console.Clear();
					var searchTerm = ConsoleHelper.ReadString("Enter a publisher to search");
					if (string.IsNullOrWhiteSpace(searchTerm)) continue;
					var results = bookManagementService.SearchBooks(searchTerm, BookSearchField.Publisher);
					DisplayBookResults(results, authorization);
					ConsoleHelper.Pause();
					break;
				}
				case 8:
				{
					ConsoleHelper.ShowInfo(string.Format(Messages.SearchCancelled, "Book"));
					return;
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


	private static void ViewBookDetails(BookManagementService bookManagementService,
		LoanManagementService loanManagementService, ICurrentUserSession session)
	{
		var desiredBook = SelectExistingBook(bookManagementService, "Book Details");
		if (desiredBook is null) return;
		BookPrinter.PrintDetails(desiredBook);

		if (session.IsSelfServiceMember)
		{
			var ownLoans = loanManagementService.GetOwnLoansByBook(desiredBook.BookId, session);
			BookPrinter.PrintLoanHistory(ownLoans);
			return;
		}

		var loans = loanManagementService.GetLoanByBook(desiredBook.BookId, session);
		BookPrinter.PrintLoanHistory(loans);
	}
}