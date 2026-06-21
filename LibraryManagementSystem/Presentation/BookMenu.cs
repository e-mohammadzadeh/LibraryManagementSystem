using LibraryManagementSystem.Domain;
using LibraryManagementSystem.Enums;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Services;

namespace LibraryManagementSystem.Presentation;

public static class BookMenu
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

	public static void BookMenuController(UserManagementService userManagementService,
		BookManagementService bookManagementService)
	{
		var continueProgram = true;
		while (continueProgram)
		{
			switch (BookMenuList())
			{
				case 1:
				{
					Console.Clear();
					AddBook(userManagementService, bookManagementService);
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
				case 5: {
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
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Backing to main menu...\n");
					Console.ResetColor();
					Thread.Sleep(3000);
					Console.Clear();
					continueProgram = false;
					break;
				}
			}
		}
	}


	private static void AddBook(UserManagementService userManagementService,
		BookManagementService bookManagementService)
	{
		Console.WriteLine("============================ ADDING BOOK MENU ============================");
		string? isbn;
		string? bookName;
		Author? author;
		DateOnly? publishDate;
		int? totalCopies;
		int? genreId;


		while (true)  // Validate ISBN
		{
			isbn = ConsoleHelper.ReadISBN("Enter ISBN for new book:");
			if (isbn == null)
				return;
			break;
		}

		while (true)  // Validate Book Name
		{
			bookName = ConsoleHelper.ReadString("Enter new book's fullname: ");
			if (bookName == null)
				return;

			if (Validator.EmailValidator(isbn)) {
				break;
			}
			Console.WriteLine("Invalid ISBN.Please try again.");
			Console.ResetColor();
		}


		while (true)
		{
			var allAuthors = userManagementService.GetAllAuthors();
			if (allAuthors.Count == 0)
			{
				Console.WriteLine("No authors found. First add new author.");
				return;
			}
			else
			{
				Console.WriteLine("{0,3} {1, 15} {2, 20}", "ID", "Author Name", "Email Address");
				Console.WriteLine("========================================================");

				foreach (var a in allAuthors) {
					var fullName = a.FirstName + a.LastName;
					Console.WriteLine("{0,3} {1, 15} {2, 20}", a.AuthorId, fullName, a.Email);
				}
				Console.WriteLine("========================================================");
				var authorId = ConsoleHelper.ReadInt("Enter your desired author's ID: ", 1, allAuthors.Count);

				author = userManagementService.FindAuthorById(authorId);

				break;
			}
		}

		while (true)
		{
			publishDate = ConsoleHelper.ReadDateOnly("Enter publish date of this book: ");
			if (publishDate == null)
				return;

			if (Validator.EmailValidator(isbn)) {
				break;
			}
			Console.WriteLine("Invalid ISBN.Please try again.");
			Console.ResetColor();
		}

		while (true)
		{
			totalCopies = ConsoleHelper.ReadInt("Enter total number of copies for this book: ", 1, 10);
			if (totalCopies == null)
				return;

			if (Validator.EmailValidator(isbn)) {
				break;
			}
			Console.WriteLine("Invalid ISBN.Please try again.");
			Console.ResetColor();
		}
		while (true)
		{
			Console.WriteLine("{0,3} {1, 15}", "ID", "Genre Name");
			Console.WriteLine("============================");
			var values = Enum.GetValues(typeof(Genre));
			for (var i = 0; i < values.Length; i++)
			{
				Console.WriteLine("{0,3} {1, 15}", i + 1, values.GetValue(i));
			}
			Console.WriteLine("============================");

			genreId = ConsoleHelper.ReadInt("Select your desired genre by entering its ID: ", 1, 11);
			if (genreId == null)
				return;
			break;
		}

		var description = ConsoleHelper.ReadString("You can add any descriptions about this book (Optional): ");


		var result = bookManagementService.AddBook(isbn, bookName, author, publishDate.Value, totalCopies.Value,
			genreId.Value, description);

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