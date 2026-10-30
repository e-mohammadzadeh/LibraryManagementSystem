using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.ValueObjects;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryBookRepository : IBookRepository
{
	private readonly List<Book> _books;


	public InMemoryBookRepository(IAuthorRepository authorRepository, ITranslatorRepository translatorRepository)
	{
		_books = CreateSeedData(authorRepository, translatorRepository);
	}


	private static List<Book> CreateSeedData(IAuthorRepository authors, ITranslatorRepository translators)
	{
		var a1 = authors.GetAll(EntityFilter.Active).First(a => a.LastName == "Orwell");
		var a2 = authors.GetAll(EntityFilter.Active).First(a => a.LastName == "Huxley");
		var a3 = authors.GetAll(EntityFilter.Active).First(a => a.LastName == "Bradbury");
		var a4 = authors.GetAll(EntityFilter.Active).First(a => a.LastName == "Rowling");

		var t1 = translators.GetAll(EntityFilter.Active).First(t => t.LastName == "Daryabandari");
		var t2 = translators.GetAll(EntityFilter.Active).First(t => t.LastName == "Golshiri");
		var t3 = translators.GetAll(EntityFilter.Active).First(t => t.LastName == "Pirnia");
		var t4 = translators.GetAll(EntityFilter.Active).First(t => t.LastName == "SeyedHosseini");

		var book1 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "1984",
			ISBN = ISBN.Create("978-0-452-28423-4"),
			PublishDate = new DateOnly(1949, 6, 8),
			TotalCopies = 5,
			Genre = BookGenre.Create(Genre.ScienceFiction),
			Publisher = "HarperCollins",
			OriginalLanguage = Language.Arabic,
			Description = "A dystopian novel."
		};
		book1.BookAuthors.Add(new BookAuthor { Book = book1, BookId = book1.Id, Author = a1, AuthorId = a1.Id });
		book1.BookTranslators.Add(new BookTranslator
			{ Book = book1, BookId = book1.Id, Translator = t1, TranslatorId = t1.Id });
		book1.BookTranslators.Add(new BookTranslator
			{ Book = book1, BookId = book1.Id, Translator = t2, TranslatorId = t2.Id });
		book1.BookTranslators.Add(new BookTranslator
			{ Book = book1, BookId = book1.Id, Translator = t3, TranslatorId = t3.Id });
		book1.BookTranslators.Add(new BookTranslator
			{ Book = book1, BookId = book1.Id, Translator = t4, TranslatorId = t4.Id });

		var book2 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "Brave New World",
			ISBN = ISBN.Create("9780060850524"),
			PublishDate = new DateOnly(1932, 1, 1),
			TotalCopies = 5,
			Genre = BookGenre.Create(Genre.ScienceFiction),
			Publisher = "Amir Kabir Publishing",
			OriginalLanguage = Language.Chinese,
			Description = "A dystopian novel."
		};
		book2.BookAuthors.Add(new BookAuthor { Book = book2, BookId = book2.Id, Author = a1, AuthorId = a1.Id });
		book2.BookAuthors.Add(new BookAuthor { Book = book2, BookId = book2.Id, Author = a2, AuthorId = a2.Id });
		book2.BookTranslators.Add(new BookTranslator
			{ Book = book2, BookId = book2.Id, Translator = t2, TranslatorId = t2.Id });

		var book3 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "Fahrenheit 451",
			ISBN = ISBN.Create("9781451673319"),
			PublishDate = new DateOnly(1953, 1, 1),
			TotalCopies = 5,
			Genre = BookGenre.Create(Genre.Horror),
			Publisher = "Oxford University Press",
			OriginalLanguage = Language.English,
			Description = "A dystopian novel."
		};
		book3.BookAuthors.Add(new BookAuthor { Book = book3, BookId = book3.Id, Author = a2, AuthorId = a2.Id });
		book3.BookAuthors.Add(new BookAuthor { Book = book3, BookId = book3.Id, Author = a3, AuthorId = a3.Id });
		book3.BookAuthors.Add(new BookAuthor { Book = book3, BookId = book3.Id, Author = a4, AuthorId = a4.Id });
		book3.BookTranslators.Add(new BookTranslator
			{ Book = book3, BookId = book3.Id, Translator = t2, TranslatorId = t2.Id });
		book3.BookTranslators.Add(new BookTranslator
			{ Book = book3, BookId = book3.Id, Translator = t3, TranslatorId = t3.Id });

		var book4 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "Harry Potter and the Philosopher's Stone",
			ISBN = ISBN.Create("978-0-7475-3269-9"),
			PublishDate = new DateOnly(1997, 6, 26),
			TotalCopies = 3,
			Genre = BookGenre.Create(Genre.Fantasy),
			Publisher = "HarperCollins",
			OriginalLanguage = Language.French,
			Description = "A young wizard discovers his magical heritage."
		};
		book4.BookAuthors.Add(new BookAuthor { Book = book4, BookId = book4.Id, Author = a2, AuthorId = a2.Id });
		book4.BookTranslators.Add(new BookTranslator
			{ Book = book4, BookId = book4.Id, Translator = t4, TranslatorId = t4.Id });

		var book5 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "Animal Farm",
			ISBN = ISBN.Create("978-0-452-28424-1"),
			PublishDate = new DateOnly(1945, 8, 17),
			TotalCopies = 4,
			Genre = BookGenre.Create(Genre.Historical),
			Publisher = "Oxford University Press",
			OriginalLanguage = Language.German,
			Description = "An allegorical novella."
		};
		book5.BookAuthors.Add(new BookAuthor { Book = book5, BookId = book5.Id, Author = a1, AuthorId = a1.Id });
		book5.BookAuthors.Add(new BookAuthor { Book = book5, BookId = book5.Id, Author = a2, AuthorId = a2.Id });
		book5.BookAuthors.Add(new BookAuthor { Book = book5, BookId = book5.Id, Author = a3, AuthorId = a3.Id });
		book5.BookAuthors.Add(new BookAuthor { Book = book5, BookId = book5.Id, Author = a4, AuthorId = a4.Id });
		book5.BookTranslators.Add(new BookTranslator
			{ Book = book5, BookId = book5.Id, Translator = t1, TranslatorId = t1.Id });

		var book6 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "To Kill a Mockingbird",
			ISBN = ISBN.Create("9780061120084"),
			PublishDate = new DateOnly(1960, 7, 11),
			TotalCopies = 6,
			Genre = BookGenre.Create(Genre.ScienceFiction),
			Publisher = "Amir Kabir Publishing",
			OriginalLanguage = Language.Italian,
			Description = "A story about racism and justice in the American South."
		};
		book6.BookAuthors.Add(new BookAuthor { Book = book6, BookId = book6.Id, Author = a4, AuthorId = a4.Id });
		book6.BookAuthors.Add(new BookAuthor { Book = book6, BookId = book6.Id, Author = a1, AuthorId = a1.Id });
		book6.BookTranslators.Add(new BookTranslator
			{ Book = book6, BookId = book6.Id, Translator = t2, TranslatorId = t2.Id });
		book6.BookTranslators.Add(new BookTranslator
			{ Book = book6, BookId = book6.Id, Translator = t3, TranslatorId = t3.Id });
		book6.BookTranslators.Add(new BookTranslator
			{ Book = book6, BookId = book6.Id, Translator = t4, TranslatorId = t4.Id });

		var book7 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "The Great Gatsby",
			ISBN = ISBN.Create("9780743273565"),
			PublishDate = new DateOnly(1925, 4, 10),
			TotalCopies = 4,
			Genre = BookGenre.Create(Genre.Horror),
			Publisher = "Oxford University Press",
			OriginalLanguage = Language.Japanese,
			Description = "A tale of wealth, love, and the American Dream."
		};
		book7.BookAuthors.Add(new BookAuthor { Book = book7, BookId = book7.Id, Author = a4, AuthorId = a4.Id });
		book7.BookTranslators.Add(new BookTranslator
			{ Book = book7, BookId = book7.Id, Translator = t3, TranslatorId = t3.Id });

		var book8 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "Pride and Prejudice",
			ISBN = ISBN.Create("9780141439518"),
			PublishDate = new DateOnly(1813, 1, 28),
			TotalCopies = 5,
			Genre = BookGenre.Create(Genre.Romance),
			Publisher = "Macmillan Publishers",
			OriginalLanguage = Language.Persian,
			Description = "A witty story about love and social class."
		};
		book8.BookAuthors.Add(new BookAuthor { Book = book8, BookId = book8.Id, Author = a4, AuthorId = a4.Id });
		book8.BookTranslators.Add(new BookTranslator
			{ Book = book8, BookId = book8.Id, Translator = t4, TranslatorId = t4.Id });

		var book9 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "The Hobbit",
			ISBN = ISBN.Create("9780547928210"),
			PublishDate = new DateOnly(1937, 9, 21),
			TotalCopies = 7,
			Genre = BookGenre.Create(Genre.Fantasy),
			Publisher = "Amir Kabir Publishing",
			OriginalLanguage = Language.Russian,
			Description = "A hobbit embarks on an unexpected journey."
		};
		book9.BookAuthors.Add(new BookAuthor { Book = book9, BookId = book9.Id, Author = a4, AuthorId = a4.Id });
		book9.BookAuthors.Add(new BookAuthor { Book = book9, BookId = book9.Id, Author = a3, AuthorId = a3.Id });
		book9.BookTranslators.Add(new BookTranslator
			{ Book = book9, BookId = book9.Id, Translator = t1, TranslatorId = t1.Id });
		book9.BookTranslators.Add(new BookTranslator
			{ Book = book9, BookId = book9.Id, Translator = t2, TranslatorId = t2.Id });

		var book10 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "Dune",
			ISBN = ISBN.Create("9780441172719"),
			PublishDate = new DateOnly(1965, 8, 1),
			TotalCopies = 4,
			Genre = BookGenre.Create(Genre.ScienceFiction),
			Publisher = "HarperCollins",
			OriginalLanguage = Language.Spanish,
			Description = "Epic science fiction on a desert planet."
		};
		book10.BookAuthors.Add(new BookAuthor { Book = book10, BookId = book10.Id, Author = a4, AuthorId = a4.Id });
		book10.BookTranslators.Add(new BookTranslator
			{ Book = book10, BookId = book10.Id, Translator = t2, TranslatorId = t2.Id });

		var book11 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "The Alchemist",
			ISBN = ISBN.Create("9780062315007"),
			PublishDate = new DateOnly(1988, 1, 1),
			TotalCopies = 8,
			Genre = BookGenre.Create(Genre.ScienceFiction),
			Publisher = "Macmillan Publishers",
			OriginalLanguage = Language.English,
			Description = "A shepherd's journey to find his personal legend."
		};
		book11.BookAuthors.Add(new BookAuthor { Book = book11, BookId = book11.Id, Author = a4, AuthorId = a4.Id });
		book11.BookTranslators.Add(new BookTranslator
			{ Book = book11, BookId = book11.Id, Translator = t3, TranslatorId = t3.Id });

		var book12 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "Sapiens: A Brief History of Humankind",
			ISBN = ISBN.Create("9780062316097"),
			PublishDate = new DateOnly(2011, 1, 1),
			TotalCopies = 3,
			Genre = BookGenre.Create(Genre.Historical),
			Publisher = "Macmillan Publishers",
			OriginalLanguage = Language.Persian,
			Description = "A groundbreaking exploration of human history."
		};
		book12.BookAuthors.Add(new BookAuthor { Book = book12, BookId = book12.Id, Author = a4, AuthorId = a4.Id });
		book12.BookTranslators.Add(new BookTranslator
			{ Book = book12, BookId = book12.Id, Translator = t4, TranslatorId = t4.Id });

		var book13 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "The Da Vinci Code",
			ISBN = ISBN.Create("9780307474278"),
			PublishDate = new DateOnly(2003, 3, 18),
			TotalCopies = 5,
			Genre = BookGenre.Create(Genre.Mystery),
			Publisher = "Oxford University Press",
			OriginalLanguage = Language.Chinese,
			Description = "A thrilling mystery involving art and secret societies."
		};
		book13.BookAuthors.Add(new BookAuthor { Book = book13, BookId = book13.Id, Author = a1, AuthorId = a1.Id });
		book13.BookAuthors.Add(new BookAuthor { Book = book13, BookId = book13.Id, Author = a2, AuthorId = a2.Id });
		book13.BookAuthors.Add(new BookAuthor { Book = book13, BookId = book13.Id, Author = a3, AuthorId = a3.Id });
		book13.BookAuthors.Add(new BookAuthor { Book = book13, BookId = book13.Id, Author = a4, AuthorId = a4.Id });
		book13.BookTranslators.Add(new BookTranslator
			{ Book = book13, BookId = book13.Id, Translator = t1, TranslatorId = t1.Id });
		book13.BookTranslators.Add(new BookTranslator
			{ Book = book13, BookId = book13.Id, Translator = t2, TranslatorId = t2.Id });
		book13.BookTranslators.Add(new BookTranslator
			{ Book = book13, BookId = book13.Id, Translator = t3, TranslatorId = t3.Id });
		book13.BookTranslators.Add(new BookTranslator
			{ Book = book13, BookId = book13.Id, Translator = t4, TranslatorId = t4.Id });

		var book14 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "Educated: A Memoir",
			ISBN = ISBN.Create("9780062420091"),
			PublishDate = new DateOnly(2018, 2, 20),
			TotalCopies = 4,
			Genre = BookGenre.Create(Genre.Biography),
			Publisher = "Amir Kabir Publishing",
			OriginalLanguage = Language.Russian,
			Description = "A story of self-invention and overcoming adversity."
		};
		book14.BookAuthors.Add(new BookAuthor { Book = book14, BookId = book14.Id, Author = a4, AuthorId = a4.Id });
		book14.BookTranslators.Add(new BookTranslator
			{ Book = book14, BookId = book14.Id, Translator = t2, TranslatorId = t2.Id });

		var book15 = new Book
		{
			Id = Guid.CreateVersion7(),
			Title = "The Silent Patient",
			ISBN = ISBN.Create("9781250301697"),
			PublishDate = new DateOnly(2019, 2, 5),
			TotalCopies = 6,
			Genre = BookGenre.Create(Genre.Thriller),
			Publisher = "Oxford University Press",
			OriginalLanguage = Language.Italian,
			Description = "A woman shoots her husband and never speaks again."
		};
		book15.BookAuthors.Add(new BookAuthor { Book = book15, BookId = book15.Id, Author = a4, AuthorId = a4.Id });
		book15.BookTranslators.Add(new BookTranslator
			{ Book = book15, BookId = book15.Id, Translator = t3, TranslatorId = t3.Id });

		return
		[
			book1, book2, book3, book4, book5, book6, book7, book8, book9, book10, book11, book12, book13, book14,
			book15
		];
	}


	public void Add(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);

		book.Id = Guid.CreateVersion7();
		book.AvailableCopies = book.TotalCopies;
		book.CreatedAt = DateTime.UtcNow;
		book.IsRemoved = false;
		_books.Add(book);
	}


	public Book? GetById(Guid id, EntityFilter filter = EntityFilter.Active)
	{
		return ApplyFilter(_books, filter).FirstOrDefault(b => b.Id == id);
	}


	public IReadOnlyList<Book> GetAll(EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_books, filter)];
	}


	public IReadOnlyList<Book> GetByAuthorId(Guid authorId, EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_books, filter).Where(b => b.BookAuthors.Any(a => a.AuthorId == authorId))];
	}


	public IReadOnlyList<Book> GetByTranslatorId(Guid translatorId, EntityFilter filter = EntityFilter.Active)
	{
		return [.. ApplyFilter(_books, filter).Where(b => b.BookTranslators.Any(t => t.TranslatorId == translatorId))];
	}


	public bool Exists<T>(T value, Func<Book, T> selector, Guid? excludeId)
	{
		return _books.Any(b =>
			!b.IsRemoved &&
			(excludeId is null || b.Id != excludeId) &&
			EqualityComparer<T>.Default.Equals(selector(b), value));
	}


	public void Remove(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);

		if (book.IsRemoved) return;
		book.IsRemoved = true;
		book.UpdatedAt = DateTime.UtcNow;
	}


	public void Update(Book book)
	{
		ArgumentNullException.ThrowIfNull(book);

		var tracked = _books.FirstOrDefault(b => b.Id == book.Id) ?? throw new BookNotFoundException(book.Id);

		tracked.Title = book.Title;
		tracked.ISBN = book.ISBN;
		tracked.BookAuthors = book.BookAuthors;
		tracked.BookTranslators = book.BookTranslators;
		tracked.PublishDate = book.PublishDate;
		tracked.Genre = book.Genre;
		tracked.Publisher = book.Publisher;
		tracked.OriginalLanguage = book.OriginalLanguage;
		tracked.TotalCopies = book.TotalCopies;
		tracked.AvailableCopies = book.AvailableCopies;
		tracked.Description = book.Description;
		tracked.UpdatedAt = DateTime.UtcNow;
		tracked.UpdatedByUserId = book.UpdatedByUserId;
	}


	// ---------- Private helper ---------
	private static IEnumerable<Book> ApplyFilter(IEnumerable<Book> source, EntityFilter filter)
	{
		return filter switch
		{
			EntityFilter.Active => source.Where(b => !b.IsRemoved),
			EntityFilter.Removed => source.Where(b => b.IsRemoved),
			EntityFilter.All => source,
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
	}
}