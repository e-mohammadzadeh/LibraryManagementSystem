using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
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
					break;
				}
				case 3:
				{
					Console.Clear();
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
			if (int.TryParse(option, out var result) && result is >= 1 and <= 7)
				return result;

			ConsoleHelper.ShowError(ValidationMessages.InvalidMenuChoice);
		}
	}


	private static void AddBook(UserManagementService userManagementService,
		BookManagementService bookManagementService)
	{
		Console.WriteLine("============================ ADDING BOOK MENU ============================");
		Author? author = null;
		DateOnly? publishDate;
		int? totalCopies;
		int? genreId;



		var isbn = ConsoleHelper.ReadISBN("Enter ISBN for new book");
		if (isbn is null)
			return;

		var bookName = ConsoleHelper.GetValidName("Enter new book's fullname", "book name",
			ValidationConstants.MinBookNameLength, ValidationConstants.MaxBookNameLength);
		if (bookName is null)
			return;

		var authorList = userManagementService.GetAllAuthors();
		if (authorList.Count != 0)
			author = MenuHelper.SelectAuthor(authorList);
	

		while (true)
		{
			publishDate = ConsoleHelper.GetValidDate("Enter publish date of this book");
			if (publishDate == null)
				return;

			break;
		}

		while (true)
		{
			totalCopies = ConsoleHelper.ReadInt("Enter total number of copies for this book", 1, 10);
			if (totalCopies == null)
				return;

			break;
		}

		while (true)
		{
			DisplayGenres();
			genreId = ConsoleHelper.ReadInt("Select your desired genre by entering its ID", 1, Enum.GetValues<Genre>().Length);
			if (genreId == null)
				return;

			break;
		}

		var description = ConsoleHelper.ReadString("You can add any descriptions about this book (Optional)");


		var result = bookManagementService.AddBook(isbn, bookName, author, publishDate.Value, totalCopies.Value,
			genreId.Value, description);
		ConsoleHelper.ShowResult(result);


		if (result.Success)
			ConsoleHelper.ShowSuccess("Book added successfully.");
		else
			ConsoleHelper.ShowError($"Failed to add book. {result.Message}");
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
}