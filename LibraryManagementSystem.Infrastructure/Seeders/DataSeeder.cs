using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;
// ReSharper disable StringLiteralTypo

namespace LibraryManagementSystem.Infrastructure.Seeders;

public static class DataSeeder
{
	public static void Seed(IAuthorRepository authorRepository, ITranslatorRepository translatorRepository,
		IBookRepository bookRepository, IUserRepository userRepository, ILoanRepository loanRepository,
		IRoleRepository roleRepository)
	{
		SeedAuthors(authorRepository, translatorRepository, bookRepository);
		SeedUsers(userRepository, roleRepository);
		//SeedRoles(roleRepository);
	}


	private static void SeedAuthors(IAuthorRepository authorRepository, ITranslatorRepository translatorRepository,
		IBookRepository bookRepository)
	{
		// Seed authors
		var author1 = new Author("George", "Orwell", "1234567890", "orwell@example.com", "09120000001",
			new DateOnly(1903, 6, 25), "English novelist and essayist.");

		var author2 = new Author("Aldous", "Huxley", "0987654321", "huxley@example.com", "09120000002",
			new DateOnly(1894, 11, 26), "English writer and social critic.");

		var author3 = new Author("Ray", "Bradbury", "1122334455", "bradbury@example.com", "09120000003",
			new DateOnly(1920, 8, 22), "American science fiction and fantasy writer.");

		var author4 = new Author("J.K.", "Rowling", "0987654321", "rowling@example.com", "09120000002",
			new DateOnly(1965, 7, 31), "British author of the Harry Potter series.");

		var translator1 = new Translator("Najaf", "Daryabandari", "1234567890", "najaf.daryabandari@example.com",
			"09123456789",
			new DateOnly(1921, 6, 12));

		var translator2 = new Translator("Ahmad", "Golshiri", "0987654321", "ahmad.golshiri@example.com", "09127654321",
			new DateOnly(1940, 3, 25));

		var translator3 = new Translator("Mansoureh", "Pirnia", "1122334455", "mansoureh.pirnia@example.com",
			"09129876543",
			new DateOnly(1955, 11, 8));

		var translator4 = new Translator("Reza", "SeyedHosseini", "6677889900", "reza.seyedhosseini@example.com",
			"09121122334",
			new DateOnly(1968, 9, 17));

		authorRepository.Add(author1);
		authorRepository.Add(author2);
		authorRepository.Add(author3);
		authorRepository.Add(author4);

		translatorRepository.Add(translator1);
		translatorRepository.Add(translator2);
		translatorRepository.Add(translator3);
		translatorRepository.Add(translator4);

		// Seed books
		var book1 = new Book("978-0-452-28423-4", "1984", author1, translator1, new DateOnly(1949, 6, 8), 5,
			Genre.ScienceFiction, "HarperCollins", "A dystopian novel.");

		var book2 = new Book("9780060850524", "Brave New World", author1, translator2, new DateOnly(1932, 1, 1), 5,
			Genre.ScienceFiction, "Amir Kabir Publishing", "A dystopian novel.");

		var book3 = new Book("9781451673319", "Fahrenheit 451", author2, translator3, new DateOnly(1953, 1, 1), 5,
			Genre.Horror, "Oxford University Press", "A dystopian novel.");

		var book4 = new Book("978-0-7475-3269-9", "Harry Potter and the Philosopher's Stone", author2, translator4,
			new DateOnly(1997, 6, 26), 3, Genre.Fantasy, "HarperCollins",
			"A young wizard discovers his magical heritage.");

		var book5 = new Book("978-0-452-28424-1", "Animal Farm", author1, translator1, new DateOnly(1945, 8, 17), 4,
			Genre.Historical, "Oxford University Press", "An allegorical novella.");

		var book6 = new Book("9780061120084", "To Kill a Mockingbird", author4, translator2, new DateOnly(1960, 7, 11),
			6, Genre.ScienceFiction, "Amir Kabir Publishing",
			"A story about racism and justice in the American South.");

		var book7 = new Book("9780743273565", "The Great Gatsby", author4, translator3, new DateOnly(1925, 4, 10), 4,
			Genre.Horror, "Oxford University Press", "A tale of wealth, love, and the American Dream.");

		var book8 = new Book("9780141439518", "Pride and Prejudice", author4, translator4, new DateOnly(1813, 1, 28), 5,
			Genre.Romance, "Macmillan Publishers", "A witty story about love and social class.");

		var book9 = new Book("9780547928210", "The Hobbit", author4, translator1, new DateOnly(1937, 9, 21), 7,
			Genre.Fantasy, "Amir Kabir Publishing", "A hobbit embarks on an unexpected journey.");

		var book10 = new Book("9780441172719", "Dune", author4, translator2, new DateOnly(1965, 8, 1), 4,
			Genre.ScienceFiction, "HarperCollins", "Epic science fiction on a desert planet.");

		var book11 = new Book("9780062315007", "The Alchemist", author4, translator3, new DateOnly(1988, 1, 1), 8,
			Genre.ScienceFiction, "Macmillan Publishers", "A shepherd's journey to find his personal legend.");

		var book12 = new Book("9780062316097", "Sapiens: A Brief History of Humankind", author4, translator4,
			new DateOnly(2011, 1, 1), 3, Genre.Historical, "Macmillan Publishers",
			"A groundbreaking exploration of human history.");

		var book13 = new Book("9780307474278", "The Da Vinci Code", author4, translator1, new DateOnly(2003, 3, 18), 5,
			Genre.Mystery, "Oxford University Press", "A thrilling mystery involving art and secret societies.");

		var book14 = new Book("9780062420091", "Educated: A Memoir", author4, translator2, new DateOnly(2018, 2, 20), 4,
			Genre.Biography, "Amir Kabir Publishing", "A story of self-invention and overcoming adversity.");

		var book15 = new Book("9781250301697", "The Silent Patient", author4, translator3, new DateOnly(2019, 2, 5), 6,
			Genre.Thriller, "Oxford University Press", "A woman shoots her husband and never speaks again.");

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


		author1.Books.Add(book1);
		author1.Books.Add(book2);
		author1.Books.Add(book5);
		author2.Books.Add(book3);
		author2.Books.Add(book4);
		author4.Books.Add(book6);
		author4.Books.Add(book7);
		author4.Books.Add(book8);
		author4.Books.Add(book9);
		author4.Books.Add(book10);
		author4.Books.Add(book11);
		author4.Books.Add(book12);
		author4.Books.Add(book13);
		author4.Books.Add(book14);
		author4.Books.Add(book15);
	}


	private static void SeedUsers(IUserRepository userRepository, IRoleRepository roleRepository)
	{
		// Seed users
		var allRoles = roleRepository.GetAllRoles();
		var adminRole = allRoles.First(r => r.Name == LibraryUserRole.Admin);
		var memberRole = allRoles.First(r => r.Name == LibraryUserRole.Member);
		var librarianRole = allRoles.First(r => r.Name == LibraryUserRole.Librarian);

		var admin = new User("Sara", "Admin", "3780254901", "admin@library.com", "09120000010",
			new DateOnly(1985, 3, 15), [adminRole]);

		var librarian = new User("Ali", "Librarian", "3780254902", "librarian@library.com", "09120000011",
			new DateOnly(1990, 5, 20), [librarianRole]);

		var librarian2 = new User("Reza", "Karimi", "3780254903", "reza.karimi@library.com", "09120000014",
			new DateOnly(1988, 11, 5), [librarianRole]);

		var librarian3 = new User("Zahra", "Rahimi", "3780254904", "zahra.rahimi@library.com", "09120000015",
			new DateOnly(1992, 4, 18), [librarianRole]);

		var member1 = new User("Mohammad", "Ahmadi", "3780254905", "m.ahmadi@example.com", "09120000012",
			new DateOnly(1998, 1, 10), [memberRole]);

		var member2 = new User("Fateme", "Hosseini", "3780254906", "f.hosseini@example.com", "09120000013",
			new DateOnly(2000, 7, 25), [memberRole]);

		var member3 = new User("Hossein", "Moradi", "3780254907", "h.moradi@example.com", "09120000016",
			new DateOnly(1995, 9, 12), [memberRole]);

		var member4 = new User("Narges", "Salehi", "3780254908", "n.salehi@example.com", "09120000017",
			new DateOnly(2001, 2, 28), [memberRole]);

		var member5 = new User("Ali", "Rezaei", "3780254909", "a.rezaei@example.com", "09120000018",
			new DateOnly(1997, 6, 15), [memberRole]);

		var member6 = new User("Maryam", "Khalili", "3780254910", "maryam.khalili@example.com", "09120000019",
			new DateOnly(1999, 10, 3), [memberRole]);

		var member7 = new User("Seyed", "Mousavi", "3780254911", "s.mousavi@example.com", "09120000020",
			new DateOnly(1987, 12, 22), [memberRole]);

		var member8 = new User("Leila", "Pourahmadi", "3780254912", "leila.pourahmadi@example.com", "09120000021",
			new DateOnly(2002, 5, 7), [memberRole]);

		var member9 = new User("Mehdi", "Hashemi", "3780254913", "mehdi.hashemi@example.com", "09120000022",
			new DateOnly(1996, 8, 19), [memberRole]);

		var member10 = new User("Sara", "Nikoo", "3780254914", "s.nikoo@example.com", "09120000023",
			new DateOnly(2003, 3, 30), [memberRole]);

		var member11 = new User("Amir", "Jafari", "3780254915", "amir.jafari@example.com", "09120000024",
			new DateOnly(1994, 7, 14), [memberRole]);

		var member12 = new User("Fatemeh", "Ebrahimi", "3780254916", "f.ebrahimi@example.com", "09120000025",
			new DateOnly(1991, 11, 9), [memberRole]);

		userRepository.Add(admin);
		userRepository.Add(librarian);
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
	}
}