using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.Presentation;

public class BookMenu
{
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

	private static int BookMenuList() {
		while (true) {
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
			if (int.TryParse(option, out var result) && result is >= 1 and <= 7) {
				return result;
			}
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Invalid selection, Try again.\n");
			Console.ResetColor();
		}
	}





	private static void AddBook(BookManagementService bookManagementService) {
		Console.WriteLine("============================ ADDING BOOK MENU ============================");

		var isbn = ConsoleHelper.ReadISBN("Enter ISBN for new book:");
		var bookName = ConsoleHelper.ReadString("Enter new book's fullname: ");
		// Get information about author of book
		var publishDate = ConsoleHelper.ReadDateOnly("Enter publish date of this book: ");
		var totalCopies = ConsoleHelper.ReadInt("Enter total number of copies for this book: ", 1, 10);
		var genre = ConsoleHelper.ReadString("Enter book's genre name: ");
		var description = ConsoleHelper.ReadString("You can add any descriptions about this book (Optional): ");

		bookManagementService.AddBook(isbn, bookName, author, publishDate, totalCopies);
	}
}


