using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryBookAuthorRepository : IBookAuthorRepository
{
	private readonly List<BookAuthor> _bookAuthors;
	public InMemoryBookAuthorRepository() { _bookAuthors = CreateSeedData(); }


	private static List<BookAuthor> CreateSeedData()
	{
		var a1 = new Author
		{
			Id = Guid.CreateVersion7(),
			FirstName = "George",
			LastName = "Orwell",
			NationalCode = "1234567890",
			Email = Email.Create("orwell@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000001"),
			BirthDate = new DateOnly(1903, 6, 25),
			Biography = "English novelist and essayist.",
			CreatedAt = DateTime.UtcNow.AddDays(-90),
			IsRemoved = false
		};
		var a2 = new Author
		{
			Id = Guid.CreateVersion7(),
			FirstName = "Aldous",
			LastName = "Huxley",
			NationalCode = "0987654321",
			Email = Email.Create("huxley@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000002"),
			BirthDate = new DateOnly(1894, 11, 26),
			Biography = "English writer and social critic.",
			CreatedAt = DateTime.UtcNow.AddDays(-90),
			IsRemoved = false
		};
		var a3 = new Author
		{
			Id = Guid.CreateVersion7(),
			FirstName = "Ray",
			LastName = "Bradbury",
			NationalCode = "1122334455",
			Email = Email.Create("bradbury@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000003"),
			BirthDate = new DateOnly(1920, 8, 22),
			Biography = "American science fiction and fantasy writer.",
			CreatedAt = DateTime.UtcNow.AddDays(-90),
			IsRemoved = false
		};
		var a4 = new Author
		{
			Id = Guid.CreateVersion7(),
			FirstName = "J.K.",
			LastName = "Rowling",
			NationalCode = "0087654321",
			Email = Email.Create("rowling@example.com"),
			PhoneNumber = PhoneNumber.Create("09120000004"),
			BirthDate = new DateOnly(1965, 7, 31),
			Biography = "British author of the Harry Potter series.",
			CreatedAt = DateTime.UtcNow.AddDays(-90),
			IsRemoved = false,
		};

		var b1 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "1984",
			ISBN = ISBN.Create("978-0-452-28423-4"),
			PublishDate = new DateOnly(1949, 6, 8),
			TotalCopies = 5,
			Genre = BookGenre.Create(Genre.ScienceFiction),
			Publisher = "HarperCollins",
			OriginalLanguage = Language.Arabic,
			Description = "A dystopian novel.",
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsRemoved = false,
			AvailableCopies = 5
		};

		var b2 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "Brave New World",
			ISBN = ISBN.Create("9780060850524"),
			PublishDate = new DateOnly(1932, 1, 1),
			TotalCopies = 5,
			Genre = BookGenre.Create(Genre.ScienceFiction),
			Publisher = "Amir Kabir Publishing",
			OriginalLanguage = Language.Chinese,
			Description = "A dystopian novel.",
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsRemoved = false,
			AvailableCopies = 5
		};

		var b3 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "Fahrenheit 451",
			ISBN = ISBN.Create("9781451673319"),
			PublishDate = new DateOnly(1953, 1, 1),
			TotalCopies = 5,
			Genre = BookGenre.Create(Genre.Horror),
			Publisher = "Oxford University Press",
			OriginalLanguage = Language.English,
			Description = "A dystopian novel.",
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsRemoved = false,
			AvailableCopies = 5
		};

		var b4 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "Harry Potter and the Philosopher's Stone",
			ISBN = ISBN.Create("978-0-7475-3269-9"),
			PublishDate = new DateOnly(1997, 6, 26),
			TotalCopies = 3,
			Genre = BookGenre.Create(Genre.Fantasy),
			Publisher = "HarperCollins",
			OriginalLanguage = Language.French,
			Description = "A young wizard discovers his magical heritage.",
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsRemoved = false,
			AvailableCopies = 3
		};

		var b5 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "Animal Farm",
			ISBN = ISBN.Create("978-0-452-28424-1"),
			PublishDate = new DateOnly(1945, 8, 17),
			TotalCopies = 4,
			Genre = BookGenre.Create(Genre.Historical),
			Publisher = "Oxford University Press",
			OriginalLanguage = Language.German,
			Description = "An allegorical novella.",
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsRemoved = false,
			AvailableCopies = 4
		};

		var b6 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "To Kill a Mockingbird",
			ISBN = ISBN.Create("9780061120084"),
			PublishDate = new DateOnly(1960, 7, 11),
			TotalCopies = 6,
			Genre = BookGenre.Create(Genre.ScienceFiction),
			Publisher = "Amir Kabir Publishing",
			OriginalLanguage = Language.Italian,
			Description = "A story about racism and justice in the American South.",
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsRemoved = false,
			AvailableCopies = 6
		};

		var b7 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "The Great Gatsby",
			ISBN = ISBN.Create("9780743273565"),
			PublishDate = new DateOnly(1925, 4, 10),
			TotalCopies = 4,
			Genre = BookGenre.Create(Genre.Horror),
			Publisher = "Oxford University Press",
			OriginalLanguage = Language.Japanese,
			Description = "A tale of wealth, love, and the American Dream.",
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsRemoved = false,
			AvailableCopies = 4
		};

		var b8 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "Pride and Prejudice",
			ISBN = ISBN.Create("9780141439518"),
			PublishDate = new DateOnly(1813, 1, 28),
			TotalCopies = 5,
			Genre = BookGenre.Create(Genre.Romance),
			Publisher = "Macmillan Publishers",
			OriginalLanguage = Language.Persian,
			Description = "A witty story about love and social class.",
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsRemoved = false,
			AvailableCopies = 5
		};

		var b9 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "The Hobbit",
			ISBN = ISBN.Create("9780547928210"),
			PublishDate = new DateOnly(1937, 9, 21),
			TotalCopies = 7,
			Genre = BookGenre.Create(Genre.Fantasy),
			Publisher = "Amir Kabir Publishing",
			OriginalLanguage = Language.Russian,
			Description = "A hobbit embarks on an unexpected journey.",
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsRemoved = false,
			AvailableCopies = 7
		};

		var b10 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "Dune",
			ISBN = ISBN.Create("9780441172719"),
			PublishDate = new DateOnly(1965, 8, 1),
			TotalCopies = 4,
			Genre = BookGenre.Create(Genre.ScienceFiction),
			Publisher = "HarperCollins",
			OriginalLanguage = Language.Spanish,
			Description = "Epic science fiction on a desert planet.",
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsRemoved = false,
			AvailableCopies = 4
		};

		var b11 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "The Alchemist",
			ISBN = ISBN.Create("9780062315007"),
			PublishDate = new DateOnly(1988, 1, 1),
			TotalCopies = 8,
			Genre = BookGenre.Create(Genre.ScienceFiction),
			Publisher = "Macmillan Publishers",
			OriginalLanguage = Language.English,
			Description = "A shepherd's journey to find his personal legend.",
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsRemoved = false,
			AvailableCopies = 8
		};

		var b12 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "Sapiens: A Brief History of Humankind",
			ISBN = ISBN.Create("9780062316097"),
			PublishDate = new DateOnly(2011, 1, 1),
			TotalCopies = 3,
			Genre = BookGenre.Create(Genre.Historical),
			Publisher = "Macmillan Publishers",
			OriginalLanguage = Language.Persian,
			Description = "A groundbreaking exploration of human history.",
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsRemoved = false,
			AvailableCopies = 3
		};

		var b13 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "The Da Vinci Code",
			ISBN = ISBN.Create("9780307474278"),
			PublishDate = new DateOnly(2003, 3, 18),
			TotalCopies = 5,
			Genre = BookGenre.Create(Genre.Mystery),
			Publisher = "Oxford University Press",
			OriginalLanguage = Language.Chinese,
			Description = "A thrilling mystery involving art and secret societies.",
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsRemoved = false,
			AvailableCopies = 5
		};

		var b14 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "Educated: A Memoir",
			ISBN = ISBN.Create("9780062420091"),
			PublishDate = new DateOnly(2018, 2, 20),
			TotalCopies = 4,
			Genre = BookGenre.Create(Genre.Biography),
			Publisher = "Amir Kabir Publishing",
			OriginalLanguage = Language.Russian,
			Description = "A story of self-invention and overcoming adversity.",
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsRemoved = false,
			AvailableCopies = 4
		};

		var b15 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "The Silent Patient",
			ISBN = ISBN.Create("9781250301697"),
			PublishDate = new DateOnly(2019, 2, 5),
			TotalCopies = 6,
			Genre = BookGenre.Create(Genre.Thriller),
			Publisher = "Oxford University Press",
			OriginalLanguage = Language.Italian,
			Description = "A woman shoots her husband and never speaks again.",
			CreatedAt = DateTime.UtcNow.AddDays(-65),
			IsRemoved = false,
			AvailableCopies = 6
		};


		var ba1 = new BookAuthor { Book = b1, BookId = b1.Id, Author = a1, AuthorId = a1.Id };
		var ba2 = new BookAuthor { Book = b2, BookId = b2.Id, Author = a1, AuthorId = a1.Id };
		var ba3 = new BookAuthor { Book = b2, BookId = b2.Id, Author = a2, AuthorId = a2.Id };
		var ba4 = new BookAuthor { Book = b3, BookId = b3.Id, Author = a2, AuthorId = a2.Id };
		var ba5 = new BookAuthor { Book = b3, BookId = b3.Id, Author = a3, AuthorId = a3.Id };
		var ba6 = new BookAuthor { Book = b3, BookId = b3.Id, Author = a4, AuthorId = a4.Id };
		var ba7 = new BookAuthor { Book = b4, BookId = b4.Id, Author = a2, AuthorId = a2.Id };
		var ba8 = new BookAuthor { Book = b5, BookId = b5.Id, Author = a1, AuthorId = a1.Id };
		var ba9 = new BookAuthor { Book = b5, BookId = b5.Id, Author = a2, AuthorId = a2.Id };
		var ba10 = new BookAuthor { Book = b5, BookId = b5.Id, Author = a3, AuthorId = a3.Id };
		var ba11 = new BookAuthor { Book = b5, BookId = b5.Id, Author = a4, AuthorId = a4.Id };
		var ba12 = new BookAuthor { Book = b6, BookId = b6.Id, Author = a4, AuthorId = a4.Id };
		var ba13 = new BookAuthor { Book = b6, BookId = b6.Id, Author = a1, AuthorId = a1.Id };
		var ba14 = new BookAuthor { Book = b7, BookId = b7.Id, Author = a4, AuthorId = a4.Id };
		var ba15 = new BookAuthor { Book = b8, BookId = b8.Id, Author = a4, AuthorId = a4.Id };
		var ba16 = new BookAuthor { Book = b9, BookId = b9.Id, Author = a4, AuthorId = a4.Id };
		var ba17 = new BookAuthor { Book = b9, BookId = b9.Id, Author = a3, AuthorId = a3.Id };
		var ba18 = new BookAuthor { Book = b10, BookId = b10.Id, Author = a4, AuthorId = a4.Id };
		var ba19 = new BookAuthor { Book = b11, BookId = b11.Id, Author = a4, AuthorId = a4.Id };
		var ba20 = new BookAuthor { Book = b12, BookId = b12.Id, Author = a4, AuthorId = a4.Id };
		var ba21 = new BookAuthor { Book = b13, BookId = b13.Id, Author = a1, AuthorId = a1.Id };
		var ba22 = new BookAuthor { Book = b13, BookId = b13.Id, Author = a2, AuthorId = a2.Id };
		var ba23 = new BookAuthor { Book = b13, BookId = b13.Id, Author = a3, AuthorId = a3.Id };
		var ba24 = new BookAuthor { Book = b13, BookId = b13.Id, Author = a4, AuthorId = a4.Id };
		var ba25 = new BookAuthor { Book = b14, BookId = b14.Id, Author = a4, AuthorId = a4.Id };
		var ba26 = new BookAuthor { Book = b15, BookId = b15.Id, Author = a4, AuthorId = a4.Id };

		return
		[
			ba1, ba2, ba3, ba4, ba5, ba6, ba7, ba8, ba9, ba10, ba11, ba12, ba13, ba14, ba15, ba16, ba17, ba18, ba19,
			ba20, ba21, ba22, ba23, ba24, ba25, ba26
		];
	}



	public void Add(BookAuthor bookAuthor)
	{
		ArgumentNullException.ThrowIfNull(bookAuthor);

		var alreadyExists = _bookAuthors.Any(ba =>
			ba.BookId == bookAuthor.BookId &&
			ba.AuthorId == bookAuthor.AuthorId);

		if (alreadyExists) throw new InvalidOperationException("This author is already associated with this book.");

		_bookAuthors.Add(bookAuthor);
	}


	public void Remove(BookAuthor bookAuthor) { ArgumentNullException.ThrowIfNull(bookAuthor); }


	public void Update(BookAuthor bookAuthor)
	{
		ArgumentNullException.ThrowIfNull(bookAuthor);

		var tracked = _bookAuthors.FirstOrDefault(ba =>
			              ba.BookId == bookAuthor.BookId &&
			              ba.AuthorId == bookAuthor.AuthorId) ??
		              throw new BookAuthorNotFoundException(bookAuthor.AuthorId, bookAuthor.BookId);

		tracked.Author = bookAuthor.Author;
		tracked.AuthorId = bookAuthor.AuthorId;
		tracked.Book = bookAuthor.Book;
		tracked.BookId = bookAuthor.BookId;
	}


	public BookAuthor? GetByBookId(Guid bookId, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_bookAuthors, filter).FirstOrDefault(ba => ba.BookId == bookId);
	}


	public BookAuthor? GetByAuthorId(Guid authorId, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_bookAuthors, filter).FirstOrDefault(ba => ba.AuthorId == authorId);
	}


	public IReadOnlyList<BookAuthor> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_bookAuthors, filter)];
	}


	public bool Exists<T>(T value, Func<BookAuthor, T> selector, Guid? excludeId)
	{
		return _bookAuthors.Any(ba =>
			!ba.Author.IsRemoved &&
			!ba.Book.IsRemoved &&
			(excludeId is null || ba.AuthorId != excludeId || ba.BookId != excludeId) &&
			EqualityComparer<T>.Default.Equals(selector(ba), value));
	}



	// ---------- Private helper ----------
	private static IEnumerable<BookAuthor> ApplyFilter(IEnumerable<BookAuthor> source, EntityFilter filter)
	{
		return filter switch
		{
			EntityFilter.Active => source.Where(ba => !ba.Author.IsRemoved && !ba.Book.IsRemoved),
			EntityFilter.Removed => source.Where(ba => ba.Author.IsRemoved && ba.Book.IsRemoved),
			EntityFilter.All => source,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
	}
}