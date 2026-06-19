using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Enums;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.Presentation;

public class BookMenu
{
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
			{
				return result;
			}
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid selection, Try again.\n");
			Console.ResetColor();
		}
	}

	public static void BookMenuController(BookManagementService bookManagementService)
	{
		var continueProgram = true;
		while (continueProgram)
		{
			switch (BookMenuList())
			{
				case 1:
				{
					AddBook(bookManagementService);
					break;
				}
				case 2:
				{
					break;
				}
				case 3:
				{
					break;
				}
				case 4:
				{
					break;
				}
				case 5:
				{
					break;
				}
				case 6:
				{
					break;
				}
				case 7:
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Backing to main menu...\n");
					Console.ResetColor();
					continueProgram = false;
					break;
				}
			}
		}
	}


	private static void AddBook(UserManagementService userManagementService, BookManagementService bookManagementService)
	{
		Console.WriteLine("============================ ADDING BOOK MENU ============================");

		var isbn = ConsoleHelper.ReadISBN("Enter ISBN for new book:");
		var bookName = ConsoleHelper.ReadString("Enter new book's fullname: ");

		//userManagementService.  find and print list of all authors
		Author author = new Author("Erfan", "Mohammadzadeh", "27906832402", "er.moadzadeh@gmail.com", "09372589496",
			new DateOnly(1999, 06, 23), "A passionate writer and researcher.");
		DateOnly? publishDate = ConsoleHelper.ReadDateOnly("Enter publish date of this book: ");
		var totalCopies = ConsoleHelper.ReadInt("Enter total number of copies for this book: ", 1, 10);

		Console.WriteLine("{0,3} {1, 15}", "ID", "Genre Name");
		Console.WriteLine("============================");
		var values = Enum.GetValues(typeof(Genre));
		for (var i = 0; i < values.Length; i++)
		{
			Console.WriteLine("{0,3} {1, 15}", i + 1, values.GetValue(i));
		}
		Console.WriteLine("============================");

		var genreId = ConsoleHelper.ReadInt("Select your desired genre by entering its ID: ", 1, 11);
		var description = ConsoleHelper.ReadString("You can add any descriptions about this book (Optional): ");


		var result =
			bookManagementService.AddBook(isbn, bookName, author, publishDate, totalCopies, genreId, description);

		if (result.Success)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Book added successfully.");
		}
		else
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Failed to add book." + result.Message);
		}
		Console.ResetColor();
	}
}