using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Enums;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.Presentation;

public static class BookMenu
{
	public static void BookMenuController(UserManagementService userManagementService,
		BookManagementService bookManagementService)
	{
		var continueProgram = true;
		while (continueProgram)
		{
			Console.Clear();
			switch (BookMenuList())
			{
				case 1:
				{
					Console.Clear();
					AddBook(userManagementService, bookManagementService);
					ConsoleHelper.ShowInfo("\nPress any key to continue...");
					Console.ReadKey(true);
					break;
				}
				case 2:
				{
					Console.Clear();
					EditBook(userManagementService, bookManagementService);
					ConsoleHelper.ShowInfo("\nPress any key to continue...");
					Console.ReadKey(true);
					break;
				}
				case 3:
				{
					Console.Clear();
					RemoveBook(bookManagementService);
					break;
				}
				case 4:
				{
					Console.Clear();

					break;
				}
				case 5:
				{
					Console.Clear();
					break;
				}
				case 6:
				{
					Console.Clear();
					break;
				}
				case 7:
				{
					ConsoleHelper.ShowError("Backing to main menu...\n");
					Thread.Sleep(2000);
					Console.Clear();
					continueProgram = false;
					break;
				}
			}
		}
	}


	private static int BookMenuList()
	{
		while (true)
		{
			Console.WriteLine("============================ BOOK MENU ============================");
			Console.WriteLine("1. Add Book");
			Console.WriteLine("2. Edit Book");
			Console.WriteLine("3. Remove Book");
			Console.WriteLine("4. Search Book");
			Console.WriteLine("5. View Book Details");
			Console.WriteLine("6. View All Books");
			Console.WriteLine("7. Back");
			Console.WriteLine("==================================================================");
			Console.Write("Please Enter a number: ");

			var option = Console.ReadLine();
			if (int.TryParse(option, out var result) && result is >= 1 and <= 7) return result;

			ConsoleHelper.ShowError(ValidationMessages.InvalidMenuChoice);
		}
	}


	private static void AddBook(UserManagementService userManagementService,
		BookManagementService bookManagementService)
	{
		Console.WriteLine("============================ ADDING BOOK MENU ============================");
		Author? author;


		var isbn = ConsoleHelper.ReadISBN("Enter ISBN for new book");
		if (isbn is null) return;

		var bookName = ConsoleHelper.GetValidName("Enter new book's fullname", "book name",
			ValidationConstants.MinBookNameLength, ValidationConstants.MaxBookNameLength);

		if (bookName is null) return;

		var authors = userManagementService.GetAllAuthors();
		if (authors.Count != 0)
		{
			author = MenuHelper.SelectAuthor(authors);
			if (author == null) return;
		}
		else
		{
			var choice = ConsoleHelper.ReadYesNo("No authors found. Do you want to create a new author now");
			if (choice == true)
			{
				var authorDto = AuthorMenu.PromptForAuthorDto();
				if (authorDto == null) return;

				var addAuthorResult = userManagementService.AddAuthor(authorDto);
				if (!addAuthorResult.Success)
				{
					ConsoleHelper.ShowError(ValidationMessages.NotAvailableAuthor);
					return;
				}

				ConsoleHelper.ShowResult(addAuthorResult);
				author = addAuthorResult.Data;
			}
			else
			{
				ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableAuthor);
				return;
			}
		}


		var publishDate = ConsoleHelper.GetValidDate("Enter publish date of this book");
		if (publishDate == null) return;


		var totalCopies = ConsoleHelper.ReadInt("Enter total number of copies for this book",
			ValidationConstants.MinBookCopies, ValidationConstants.MaxBookCopies);

		if (totalCopies == null) return;


		DisplayGenres();
		var genreId = ConsoleHelper.ReadInt("Select your desired genre by entering its ID", 1,
			Enum.GetValues<Genre>().Length + 1);

		if (genreId == null) return;


		var description = ConsoleHelper.ReadString("You can add any descriptions about this book (Optional)");


		var result = bookManagementService.AddBook(new CreateBookDto
		{
			ISBN = isbn,
			BookName = bookName,
			AuthorId = author.Id,
			PublishDate = publishDate.Value,
			TotalCopies = totalCopies.Value,
			GenreId = genreId.Value - 1,
			Description = description
		});

		ConsoleHelper.ShowResult(result);
	}


	private static void EditBook(UserManagementService userManagementService,
		BookManagementService bookManagementService)
	{
		Console.WriteLine("============================ EDITING BOOK MENU ============================");
		var desiredBook = SelectExistingBook(bookManagementService);
		if (desiredBook is null) return;

		while (true)
		{
			Console.WriteLine("{0, -30} [{1}]", "1. Book Name", desiredBook.BookName);
			Console.WriteLine("{0, -30} [{1}]", "2. ISBN", desiredBook.InternationalStandardBookNumber);
			Console.WriteLine("{0, -30} [{1}]", "3. Author",
				desiredBook.Author.FirstName + " " + desiredBook.Author.LastName);

			Console.WriteLine("{0, -30} [{1}]", "4. Publish Date", desiredBook.PublishDate);
			Console.WriteLine("{0, -10} [{1}]", "5. Total Copies", desiredBook.TotalCopies);
			Console.WriteLine("{0, -30} [{1}]", "6. Genre", desiredBook.Genre);
			Console.WriteLine("{0, -30} [{1}]", "7. Description", desiredBook.Description);
			Console.WriteLine("8. Cancel");
			var editMenuChoice = ConsoleHelper.ReadInt("Enter the number of the field you wish to edit", 1, 8);
			if (editMenuChoice == null) return;

			switch (editMenuChoice)
			{
				case 1:
				{
					var bookName = ConsoleHelper.GetValidName("Enter the new book name", "book name",
						ValidationConstants.MinBookNameLength, ValidationConstants.MaxBookNameLength);

					if (!PerformUpdate(bookManagementService, desiredBook.BookId, bookName,
						    v => new UpdateBookDto { BookName = v }))
						break;

					break;
				}
				case 2:
				{
					var isbn = ConsoleHelper.ReadISBN("Enter the new ISBN");
					if (!PerformUpdate(bookManagementService, desiredBook.BookId, isbn,
						    v => new UpdateBookDto { ISBN = v }))
						break;

					break;
				}
				case 3:
				{
					var authors = userManagementService.GetAllAuthors();
					if (authors.Count != 0)
					{
						var author = MenuHelper.SelectAuthor(authors);
						if (!PerformUpdate(bookManagementService, desiredBook.BookId, author,
							    v => new UpdateBookDto { AuthorId = v.Id }))
							break;
					}
					else
						ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableAuthor);

					break;
				}
				case 4:
				{
					var publishDate = ConsoleHelper.GetValidDate("Enter the new publish date");
					if (!PerformUpdate(bookManagementService, desiredBook.BookId, publishDate,
						    v => new UpdateBookDto { PublishDate = v }))
						break;

					break;
				}
				case 5:
				{
					var totalCopies = ConsoleHelper.ReadInt("Enter the new total copies",
						ValidationConstants.MinBookCopies, ValidationConstants.MaxBookCopies);

					if (!PerformUpdate(bookManagementService, desiredBook.BookId, totalCopies,
						    v => new UpdateBookDto { TotalCopies = v }))
						break;

					break;
				}
				case 6:
				{
					DisplayGenres();
					var genreId =
						ConsoleHelper.ReadInt("Enter the new genre id", 1, Enum.GetValues<Genre>().Length + 1);

					if (genreId is null) break;

					if (!PerformUpdate(bookManagementService, desiredBook.BookId, genreId - 1,
						    v => new UpdateBookDto { GenreId = v }))
						break;

					break;
				}
				case 7:
				{
					var description = ConsoleHelper.ReadString("Enter the new description");
					if (!PerformUpdate(bookManagementService, desiredBook.BookId, description,
						    v => new UpdateBookDto { Description = v }))
						break;

					break;
				}
				case 8:
				{
					ConsoleHelper.ShowError("Edit cancelled. Returning to Author Menu...");
					Thread.Sleep(3000);
					Console.Clear();
					return;
				}
			}

			var choice = ConsoleHelper.ReadYesNo("Do you want to edit another field");
			if (choice == true)
			{
				Console.Clear();
				continue;
			}

			Console.Clear();
			return;
		}
	}


	private static void DisplayGenres()
	{
		Console.WriteLine("\n{0,3} {1, 15}", "ID", "Genre Name");
		Console.WriteLine("============================");
		var values = Enum.GetValues<Genre>();
		for (var i = 0; i < values.Length; i++)
		{
			Console.WriteLine("{0,3} {1, 15}", i + 1, values.GetValue(i));
		}

		Console.WriteLine("============================");
	}


	private static Book? SelectExistingBook(BookManagementService bookManagementService)
	{
		var bookList = bookManagementService.GetAllBooks();
		if (bookList.Count != 0)
			return MenuHelper.SelectBook(bookList);

		ConsoleHelper.ShowWarning(ValidationMessages.NotAvailableBook);
		return null;
	}


	private static bool PerformUpdate<T>(BookManagementService bookManagementService, int desiredBookId,
		T? newValue, Func<T, UpdateBookDto> buildDto)
	{
		if (newValue is null) return false;

		var dto = buildDto(newValue);
		var result = bookManagementService.UpdateBook(desiredBookId, dto);
		ConsoleHelper.ShowResult(result);
		return result.Success;
	}


	private static void RemoveBook(BookManagementService bookManagementService)
	{
		Console.WriteLine("============================ REMOVING BOOK MENU ============================");
		var desiredBook = SelectExistingBook(bookManagementService);
		if (desiredBook is null)
		{
			ConsoleHelper.ShowInfo("\nPress any key to continue...");
			Console.ReadKey(true);
			return;
		}

		BookPrinter.Print(desiredBook);
		var choice = ConsoleHelper.ReadYesNo($"Are you sure you want to remove {desiredBook.BookName}");

		if (choice != true) return;
		var result = bookManagementService.RemoveBook(desiredBook.BookId);
		ConsoleHelper.ShowResult(result);
	}
}