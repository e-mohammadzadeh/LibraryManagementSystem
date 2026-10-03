using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.ValueObjects;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

// ReSharper disable StringLiteralTypo

namespace LibraryManagementSystem.Application.Seeders;

public static class DataSeeder
{
	public static void Seed(IAuthorRepository authorRepository, ITranslatorRepository translatorRepository,
		IBookRepository bookRepository, IUserRepository userRepository, ILoanRepository loanRepository,
		IRoleRepository roleRepository, IFineRepository fineRepository, IPasswordHasher passwordHasher)
	{
		SeedAuthors(authorRepository, translatorRepository, bookRepository);
		SeedUsers(userRepository, roleRepository, passwordHasher);
		SeedLoans(userRepository, bookRepository, loanRepository, fineRepository);
		//SeedFines(loanRepository, fineRepository);
	}


	private static void SeedAuthors(IAuthorRepository authorRepository, ITranslatorRepository translatorRepository,
		IBookRepository bookRepository)
	{
		// Seed authors
		var author1 = new Author
		{
			FirstName = "George",
			LastName = "Orwell",
			NationalCode = "1234567890",
			Email = Email.Create("orwell@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000001"),
			BirthDate = new DateOnly(1903, 6, 25),
			Biography = "English novelist and essayist."
		};


		var author2 = new Author
		{
			FirstName = "Aldous",
			LastName = "Huxley",
			NationalCode = "0987654321",
			Email = Email.Create("huxley@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000002"),
			BirthDate = new DateOnly(1894, 11, 26),
			Biography = "English writer and social critic."
		};

		var author3 = new Author
		{
			FirstName = "Ray",
			LastName = "Bradbury",
			NationalCode = "1122334455",
			Email = Email.Create("bradbury@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000003"),
			BirthDate = new DateOnly(1920, 8, 22),
			Biography = "American science fiction and fantasy writer."
		};

		var author4 = new Author
		{
			FirstName = "J.K.",
			LastName = "Rowling",
			NationalCode = "0087654321",
			Email = Email.Create("rowling@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000004"),
			BirthDate = new DateOnly(1965, 7, 31),
			Biography = "British author of the Harry Potter series."
		};

		var translator1 = new Translator
		{
			FirstName = "Najaf",
			LastName = "Daryabandari",
			NationalCode = "1234567890",
			Email = Email.Create("najaf.daryabandari@example.com"),
			PhoneNumber = PhoneNumber.Create("09123456789"),
			BirthDate = new DateOnly(1921, 6, 12),
			Biography = "Good Iranian translator."
		};

		var translator2 = new Translator
		{
			FirstName = "Ahmad",
			LastName = "Golshiri",
			NationalCode = "0987654321",
			Email = Email.Create("ahmad.golshiri@example.com"),
			PhoneNumber = PhoneNumber.Create("09127654321"),
			BirthDate = new DateOnly(1940, 3, 25),
			Biography = "Good Iranian translator."
		};

		var translator3 = new Translator
		{
			FirstName = "Mansoureh",
			LastName = "Pirnia",
			NationalCode = "1122334455",
			Email = Email.Create("mansoureh.pirnia@example.com"),
			PhoneNumber = PhoneNumber.Create("09129876543"),
			BirthDate = new DateOnly(1955, 11, 8),
			Biography = "Good Iranian translator."
		};

		var translator4 = new Translator
		{
			FirstName = "Reza",
			LastName = "SeyedHosseini",
			NationalCode = "6677889900",
			Email = Email.Create("reza.seyedhosseini@example.com"),
			PhoneNumber = PhoneNumber.Create("09121122334"),
			BirthDate = new DateOnly(1968, 9, 17),
			Biography = "Good Iranian translator."
		};

		authorRepository.Add(author1);
		authorRepository.Add(author2);
		authorRepository.Add(author3);
		authorRepository.Add(author4);

		translatorRepository.Add(translator1);
		translatorRepository.Add(translator2);
		translatorRepository.Add(translator3);
		translatorRepository.Add(translator4);

		// Seed books
		var book1 = new Book
		{
			Title = "1984",
			ISBN = ISBN.Create("978-0-452-28423-4"),
			PublishDate = new DateOnly(1949, 6, 8),
			TotalCopies = 5,
			Genre = BookGenre.Create(Genre.ScienceFiction),
			Publisher = "HarperCollins",
			OriginalLanguage = Language.Arabic,
			Description = "A dystopian novel."
		};
		bookRepository.AssignAuthorsToBook(book1, [author1]);
		bookRepository.AssignTranslatorsToBook(book1,
			[translator1, translator2, translator3, translator4], Language.Chinese);

		var book2 = new Book
		{
			Title = "Brave New World",
			ISBN = ISBN.Create("9780060850524"),
			PublishDate = new DateOnly(1932, 1, 1),
			TotalCopies = 5,
			Genre = BookGenre.Create(Genre.ScienceFiction),
			Publisher = "Amir Kabir Publishing",
			OriginalLanguage = Language.Chinese,
			Description = "A dystopian novel."
		};
		bookRepository.AssignAuthorsToBook(book2, [author1, author2]);
		bookRepository.AssignTranslatorsToBook(book2, [translator2], Language.English);

		var book3 = new Book
		{
			Title = "Fahrenheit 451",
			ISBN = ISBN.Create("9781451673319"),
			PublishDate = new DateOnly(1953, 1, 1),
			TotalCopies = 5,
			Genre = BookGenre.Create(Genre.Horror),
			Publisher = "Oxford University Press",
			OriginalLanguage = Language.English,
			Description = "A dystopian novel."
		};
		bookRepository.AssignAuthorsToBook(book3, [author2, author3, author4]);
		bookRepository.AssignTranslatorsToBook(book3, [translator3, translator2], Language.French);

		var book4 = new Book
		{
			Title = "Harry Potter and the Philosopher's Stone",
			ISBN = ISBN.Create("978-0-7475-3269-9"),
			PublishDate = new DateOnly(1997, 6, 26),
			TotalCopies = 3,
			Genre = BookGenre.Create(Genre.Fantasy),
			Publisher = "HarperCollins",
			OriginalLanguage = Language.French,
			Description = "A young wizard discovers his magical heritage."
		};
		bookRepository.AssignAuthorsToBook(book4, [author2]);
		bookRepository.AssignTranslatorsToBook(book4, [translator4], Language.German);

		var book5 = new Book
		{
			Title = "Animal Farm",
			ISBN = ISBN.Create("978-0-452-28424-1"),
			PublishDate = new DateOnly(1945, 8, 17),
			TotalCopies = 4,
			Genre = BookGenre.Create(Genre.Historical),
			Publisher = "Oxford University Press",
			OriginalLanguage = Language.German,
			Description = "An allegorical novella."
		};
		bookRepository.AssignAuthorsToBook(book5, [author1, author2, author3, author4]);
		bookRepository.AssignTranslatorsToBook(book5, [translator1], Language.Italian);

		var book6 = new Book
		{
			Title = "To Kill a Mockingbird",
			ISBN = ISBN.Create("9780061120084"),
			PublishDate = new DateOnly(1960, 7, 11),
			TotalCopies = 6,
			Genre = BookGenre.Create(Genre.ScienceFiction),
			Publisher = "Amir Kabir Publishing",
			OriginalLanguage = Language.Italian,
			Description = "A story about racism and justice in the American South."
		};
		bookRepository.AssignAuthorsToBook(book6, [author4, author1]);
		bookRepository.AssignTranslatorsToBook(book6, [translator2, translator3, translator4], Language.Japanese);

		var book7 = new Book
		{
			Title = "The Great Gatsby",
			ISBN = ISBN.Create("9780743273565"),
			PublishDate = new DateOnly(1925, 4, 10),
			TotalCopies = 4,
			Genre = BookGenre.Create(Genre.Horror),
			Publisher = "Oxford University Press",
			OriginalLanguage = Language.Japanese,
			Description = "A tale of wealth, love, and the American Dream."
		};
		bookRepository.AssignAuthorsToBook(book7, [author4]);
		bookRepository.AssignTranslatorsToBook(book7, [translator3], Language.Persian);

		var book8 = new Book
		{
			Title = "Pride and Prejudice",
			ISBN = ISBN.Create("9780141439518"),
			PublishDate = new DateOnly(1813, 1, 28),
			TotalCopies = 5,
			Genre = BookGenre.Create(Genre.Romance),
			Publisher = "Macmillan Publishers",
			OriginalLanguage = Language.Persian,
			Description = "A witty story about love and social class."
		};
		bookRepository.AssignAuthorsToBook(book8, [author4]);
		bookRepository.AssignTranslatorsToBook(book8, [translator4], Language.Russian);

		var book9 = new Book
		{
			Title = "The Hobbit",
			ISBN = ISBN.Create("9780547928210"),
			PublishDate = new DateOnly(1937, 9, 21),
			TotalCopies = 7,
			Genre = BookGenre.Create(Genre.Fantasy),
			Publisher = "Amir Kabir Publishing",
			OriginalLanguage = Language.Russian,
			Description = "A hobbit embarks on an unexpected journey."
		};
		bookRepository.AssignAuthorsToBook(book9, [author4, author3]);
		bookRepository.AssignTranslatorsToBook(book9, [translator1, translator2], Language.Spanish);

		var book10 = new Book
		{
			Title = "Dune",
			ISBN = ISBN.Create("9780441172719"),
			PublishDate = new DateOnly(1965, 8, 1),
			TotalCopies = 4,
			Genre = BookGenre.Create(Genre.ScienceFiction),
			Publisher = "HarperCollins",
			OriginalLanguage = Language.Spanish,
			Description = "Epic science fiction on a desert planet."
		};
		bookRepository.AssignAuthorsToBook(book10, [author4]);
		bookRepository.AssignTranslatorsToBook(book10, [translator2], Language.Arabic);

		var book11 = new Book
		{
			Title = "The Alchemist",
			ISBN = ISBN.Create("9780062315007"),
			PublishDate = new DateOnly(1988, 1, 1),
			TotalCopies = 8,
			Genre = BookGenre.Create(Genre.ScienceFiction),
			Publisher = "Macmillan Publishers",
			OriginalLanguage = Language.English,
			Description = "A shepherd's journey to find his personal legend."
		};
		bookRepository.AssignAuthorsToBook(book11, [author4]);
		bookRepository.AssignTranslatorsToBook(book11, [translator3], Language.Persian);

		var book12 = new Book
		{
			Title = "Sapiens: A Brief History of Humankind",
			ISBN = ISBN.Create("9780062316097"),
			PublishDate = new DateOnly(2011, 1, 1),
			TotalCopies = 3,
			Genre = BookGenre.Create(Genre.Historical),
			Publisher = "Macmillan Publishers",
			OriginalLanguage = Language.Persian,
			Description = "A groundbreaking exploration of human history."
		};
		bookRepository.AssignAuthorsToBook(book12, [author4]);
		bookRepository.AssignTranslatorsToBook(book12, [translator4], Language.Arabic);

		var book13 = new Book
		{
			Title = "The Da Vinci Code",
			ISBN = ISBN.Create("9780307474278"),
			PublishDate = new DateOnly(2003, 3, 18),
			TotalCopies = 5,
			Genre = BookGenre.Create(Genre.Mystery),
			Publisher = "Oxford University Press",
			OriginalLanguage = Language.Chinese,
			Description = "A thrilling mystery involving art and secret societies."
		};
		bookRepository.AssignAuthorsToBook(book13, [author1, author2, author3, author4]);
		bookRepository.AssignTranslatorsToBook(book13,
			[translator1, translator2, translator3, translator4], Language.Japanese);

		var book14 = new Book
		{
			Title = "Educated: A Memoir",
			ISBN = ISBN.Create("9780062420091"),
			PublishDate = new DateOnly(2018, 2, 20),
			TotalCopies = 4,
			Genre = BookGenre.Create(Genre.Biography),
			Publisher = "Amir Kabir Publishing",
			OriginalLanguage = Language.Russian,
			Description = "A story of self-invention and overcoming adversity."
		};
		bookRepository.AssignAuthorsToBook(book14, [author4]);
		bookRepository.AssignTranslatorsToBook(book14, [translator2], Language.German);

		var book15 = new Book
		{
			Title = "The Silent Patient",
			ISBN = ISBN.Create("9781250301697"),
			PublishDate = new DateOnly(2019, 2, 5),
			TotalCopies = 6,
			Genre = BookGenre.Create(Genre.Thriller),
			Publisher = "Oxford University Press",
			OriginalLanguage = Language.Italian,
			Description = "A woman shoots her husband and never speaks again."
		};
		bookRepository.AssignAuthorsToBook(book15, [author4]);
		bookRepository.AssignTranslatorsToBook(book15, [translator3], Language.Spanish);

		bookRepository.Add(book1);
		bookRepository.Add(book2);
		bookRepository.Add(book3);
		bookRepository.Add(book4);
		bookRepository.Add(book5);
		bookRepository.Add(book6);
		bookRepository.Add(book7);
		bookRepository.Add(book8);
		bookRepository.Add(book9);
		bookRepository.Add(book10);
		bookRepository.Add(book11);
		bookRepository.Add(book12);
		bookRepository.Add(book13);
		bookRepository.Add(book14);
		bookRepository.Add(book15);
	}


	private static void SeedUsers(IUserRepository userRepository, IRoleRepository roleRepository,
		IPasswordHasher passwordHasher)
	{
		// Seed users
		var allRoles = roleRepository.GetAllRoles();
		var adminRole = allRoles.First(r => r.Name == LibraryUserRole.Admin);
		var memberRole = allRoles.First(r => r.Name == LibraryUserRole.Member);
		var librarianRole = allRoles.First(r => r.Name == LibraryUserRole.Librarian);

		var admin = new User
		{
			FirstName = "Sara",
			LastName = "Admin",
			NationalCode = "3780254901",
			Email = Email.Create("admin@library.com"),
			PhoneNumber = PhoneNumber.Create("09120000010"),
			BirthDate = new DateOnly(1985, 3, 15),
			Role = adminRole
		};
		SetPassword(admin, "Admin@123");

		var librarian1 = new User
		{
			FirstName = "Ali",
			LastName = "Librarian",
			NationalCode = "3780254902",
			Email = Email.Create("librarian@library.com"),
			PhoneNumber = PhoneNumber.Create("09120000011"),
			BirthDate = new DateOnly(1990, 5, 20),
			Role = librarianRole
		};
		SetPassword(librarian1, "Librarian1@123");

		var librarian2 = new User
		{
			FirstName = "Reza",
			LastName = "Karimi",
			NationalCode = "3780254903",
			Email = Email.Create("reza.karimi@library.com"),
			PhoneNumber = PhoneNumber.Create("09120000014"),
			BirthDate = new DateOnly(1988, 11, 5),
			Role = librarianRole,
			MembershipStartDate = new DateOnly(2026, 1, 15)
		};
		SetPassword(librarian2, "Librarian2@123");

		var librarian3 = new User
		{
			FirstName = "Zahra",
			LastName = "Rahimi",
			NationalCode = "3780254904",
			Email = Email.Create("zahra.rahimi@library.com"),
			PhoneNumber = PhoneNumber.Create("09120000015"),
			BirthDate = new DateOnly(1992, 4, 18),
			Role = librarianRole
		};
		SetPassword(librarian3, "Librarian3@123");

		var member1 = new User
		{
			FirstName = "Mohammad",
			LastName = "Ahmadi",
			NationalCode = "3780254905",
			Email = Email.Create("m.ahmadi@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000012"),
			BirthDate = new DateOnly(1998, 1, 10),
			Role = memberRole
		};
		SetPassword(member1, "Member1@123");

		var member2 = new User
		{
			FirstName = "Fateme",
			LastName = "Hosseini",
			NationalCode = "3780254906",
			Email = Email.Create("f.hosseini@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000013"),
			BirthDate = new DateOnly(2000, 7, 25),
			Role = memberRole,
			MembershipStartDate = new DateOnly(2026, 9, 1)
		};

		var member3 = new User
		{
			FirstName = "Hossein",
			LastName = "Moradi",
			NationalCode = "3780254907",
			Email = Email.Create("h.moradi@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000016"),
			BirthDate = new DateOnly(1995, 9, 12),
			Role = memberRole
		};

		var member4 = new User
		{
			FirstName = "Narges",
			LastName = "Salehi",
			NationalCode = "3780254908",
			Email = Email.Create("n.salehi@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000017"),
			BirthDate = new DateOnly(2001, 2, 28),
			Role = memberRole,
			MembershipStartDate = new DateOnly(2025, 10, 1)
		};
		SetPassword(member4, "Member4@123");

		var member5 = new User
		{
			FirstName = "Ali",
			LastName = "Rezaei",
			NationalCode = "3780254909",
			Email = Email.Create("a.rezaei@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000018"),
			BirthDate = new DateOnly(1997, 6, 15),
			Role = memberRole
		};

		var member6 = new User
		{
			FirstName = "Maryam",
			LastName = "Khalili",
			NationalCode = "3780254910",
			Email = Email.Create("maryam.khalili@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000019"),
			BirthDate = new DateOnly(1999, 10, 3),
			Role = memberRole,
			MembershipStartDate = new DateOnly(2025, 9, 10)
		};

		var member7 = new User
		{
			FirstName = "Seyed",
			LastName = "Mousavi",
			NationalCode = "3780254911",
			Email = Email.Create("s.mousavi@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000020"),
			BirthDate = new DateOnly(1987, 12, 22),
			Role = memberRole
		};

		var member8 = new User
		{
			FirstName = "Leila",
			LastName = "Pourahmadi",
			NationalCode = "3780254912",
			Email = Email.Create("leila.pourahmadi@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000021"),
			BirthDate = new DateOnly(2002, 5, 7),
			Role = memberRole,
			MembershipStartDate = new DateOnly(2026, 10, 1)
		};

		var member9 = new User
		{
			FirstName = "Mehdi",
			LastName = "Hashemi",
			NationalCode = "3780254913",
			Email = Email.Create("mehdi.hashemi@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000022"),
			BirthDate = new DateOnly(1996, 8, 19),
			Role = memberRole
		};

		var member10 = new User
		{
			FirstName = "Sara",
			LastName = "Nikoo",
			NationalCode = "3780254914",
			Email = Email.Create("s.nikoo@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000023"),
			BirthDate = new DateOnly(2003, 3, 30),
			Role = memberRole,
			MembershipStartDate = new DateOnly(2025, 12, 1)
		};

		var member11 = new User
		{
			FirstName = "Amir",
			LastName = "Jafari",
			NationalCode = "3780254915",
			Email = Email.Create("amir.jafari@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000024"),
			BirthDate = new DateOnly(1994, 7, 14),
			Role = memberRole
		};

		var member12 = new User
		{
			FirstName = "Fatemeh",
			LastName = "Ebrahimi",
			NationalCode = "3780254916",
			Email = Email.Create("f.ebrahimi@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000025"),
			BirthDate = new DateOnly(1991, 11, 9),
			Role = memberRole,
			MembershipStartDate = new DateOnly(2026, 1, 1)
		};

		userRepository.Add(admin);
		userRepository.Add(librarian1);
		userRepository.Add(librarian2);
		userRepository.Add(librarian3);
		userRepository.Add(member1);
		userRepository.Add(member2);
		userRepository.Add(member3);
		userRepository.Add(member4);
		userRepository.Add(member5);
		userRepository.Add(member6);
		userRepository.Add(member7);
		userRepository.Add(member8);
		userRepository.Add(member9);
		userRepository.Add(member10);
		userRepository.Add(member11);
		userRepository.Add(member12);
		return;

		void SetPassword(User user, string password)
		{
			var result = passwordHasher.CreatePasswordHash(password);
			userRepository.SetPasswordHash(user, result.Hash, result.Salt);
		}
	}



	private static void SeedLoans(IUserRepository userRepository, IBookRepository bookRepository,
		ILoanRepository loanRepository, IFineRepository fineRepository)
	{
		var users = userRepository.GetAll(EntityFilter.Active);
		var books = bookRepository.GetAll(EntityFilter.Active);
		var today = DateOnly.FromDateTime(DateTime.Today);

		CreateActiveLoan(GetUser(4), GetBook(0), loanRepository, bookRepository);
		CreateActiveLoan(GetUser(4), GetBook(1), loanRepository, bookRepository);
		CreateActiveLoan(GetUser(5), GetBook(5), loanRepository, bookRepository);
		CreateActiveLoan(GetUser(6), GetBook(2), loanRepository, bookRepository);
		CreateActiveLoan(GetUser(6), GetBook(8), loanRepository, bookRepository);
		CreateActiveLoan(GetUser(6), GetBook(10), loanRepository, bookRepository);
		CreateActiveLoan(GetUser(8), GetBook(3), loanRepository, bookRepository);
		CreateActiveLoan(GetUser(7), GetBook(4), loanRepository, bookRepository);
		CreateActiveLoan(GetUser(4), GetBook(10), loanRepository, bookRepository);


		CreateReturnedLoan(GetUser(4), GetBook(6), loanRepository, bookRepository, fineRepository, today.AddDays(-25),
			today.AddDays(-5));
		CreateReturnedLoan(GetUser(7), GetBook(9), loanRepository, bookRepository, fineRepository, today.AddDays(-15),
			today.AddDays(-2));
		CreateReturnedLoan(GetUser(9), GetBook(7), loanRepository, bookRepository, fineRepository, today.AddDays(-35),
			today.AddDays(-10));
		CreateReturnedLoan(GetUser(9), GetBook(11), loanRepository, bookRepository, fineRepository, today.AddDays(-22),
			today);
		CreateReturnedLoan(GetUser(4), GetBook(12), loanRepository, bookRepository, fineRepository, today.AddDays(-63),
			today.AddDays(-5));
		return;


		// Helper to get user / book by index (safer than hard-coded IDs)
		User GetUser(int index) => users[index];
		Book GetBook(int index) => books[index];
	}


	private static void CreateActiveLoan(User user, Book book, ILoanRepository loanRepository,
		IBookRepository bookRepository)
	{
		if (book.AvailableCopies <= 0) return;

		var loan = new Loan
		{
			Book = book,
			BookId = book.Id,
			User = user,
			UserId = user.Id,
		};
		bookRepository.BorrowCopy(book);
		loanRepository.Add(loan);
	}


	private static void CreateReturnedLoan(User user, Book book, ILoanRepository loanRepository,
		IBookRepository bookRepository, IFineRepository fineRepository, DateOnly? loanDate, DateOnly returnDate)
	{
		var loan = new Loan
		{
			Book = book,
			BookId = book.Id,
			User = user,
			UserId = user.Id,
		};
		bookRepository.BorrowCopy(book);
		loan.MarkAsReturned(returnDate);
		bookRepository.ReturnCopy(book);
		loanRepository.Add(loan);
		if (!(loan.ReturnDate > loan.DueDate)) return;
		var fine = new Fine
		{
			Loan = loan,
			LoanId = loan.Id,
			UserId = loan.UserId,
			OverdueDays = loan.ReturnDate.Value.DayNumber - loan.DueDate.DayNumber,
		};

		fineRepository.Add(fine);
	}
}