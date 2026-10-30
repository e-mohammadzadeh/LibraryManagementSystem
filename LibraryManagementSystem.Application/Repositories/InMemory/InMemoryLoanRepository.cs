using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Common;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;

namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryLoanRepository : ILoanRepository
{
	private readonly List<Loan> _loans;


	public InMemoryLoanRepository(IUserRepository users, IBookRepository books)
	{
		_loans = CreateSeedData(users, books);
	}


	private static List<Loan> CreateSeedData(IUserRepository users, IBookRepository books)
	{
		var member4 = users.GetAll(EntityFilter.Active).First(u => u.Email.Value == "m.ahmadi@example.com");
		var member5 = users.GetAll(EntityFilter.Active).First(u => u.Email.Value == "f.hosseini@example.com");
		var member6 = users.GetAll(EntityFilter.Active).First(u => u.Email.Value == "h.moradi@example.com");
		var member7 = users.GetAll(EntityFilter.Active).First(u => u.Email.Value == "n.salehi@example.com");
		var member8 = users.GetAll(EntityFilter.Active).First(u => u.Email.Value == "a.rezaei@example.com");
		var member9 = users.GetAll(EntityFilter.Active).First(u => u.Email.Value == "maryam.khalili@example.com");
		var today = DateOnly.FromDateTime(DateTime.Today);

		var book1 = books.GetAll(EntityFilter.Active).First(b => b.Title == "1984");
		if (book1.AvailableCopies > 0) book1.AvailableCopies--;
		var loan1 = new Loan
		{
			User = member4,
			UserId = member4.Id,
			Book = book1,
			BookId = book1.Id,
			BorrowDate = DateOnly.FromDateTime(DateTime.Today)
		};

		var book2 = books.GetAll(EntityFilter.Active).First(b => b.Title == "Brave New World");
		if (book2.AvailableCopies > 0) book2.AvailableCopies--;
		var loan2 = new Loan
		{
			User = member4,
			UserId = member4.Id,
			Book = book2,
			BookId = book2.Id,
			BorrowDate = DateOnly.FromDateTime(DateTime.Today)
		};

		var book3 = books.GetAll(EntityFilter.Active).First(b => b.Title == "Fahrenheit 451");
		if (book3.AvailableCopies > 0) book3.AvailableCopies--;
		var loan3 = new Loan
		{
			User = member6,
			UserId = member6.Id,
			Book = book3,
			BookId = book3.Id,
			BorrowDate = DateOnly.FromDateTime(DateTime.Today)
		};


		var book4 = books.GetAll(EntityFilter.Active).First(b => b.Title == "Harry Potter and the Philosopher's Stone");
		if (book4.AvailableCopies > 0) book4.AvailableCopies--;
		var loan4 = new Loan
		{
			User = member8,
			UserId = member8.Id,
			Book = book4,
			BookId = book4.Id,
			BorrowDate = DateOnly.FromDateTime(DateTime.Today)
		};


		var book5 = books.GetAll(EntityFilter.Active).First(b => b.Title == "Animal Farm");
		if (book5.AvailableCopies > 0) book5.AvailableCopies--;
		var loan5 = new Loan
		{
			User = member7,
			UserId = member7.Id,
			Book = book5,
			BookId = book5.Id,
			BorrowDate = DateOnly.FromDateTime(DateTime.Today)
		};


		var book6 = books.GetAll(EntityFilter.Active).First(b => b.Title == "To Kill a Mockingbird");
		if (book6.AvailableCopies > 0) book6.AvailableCopies--;
		var loan6 = new Loan
		{
			User = member5,
			UserId = member5.Id,
			Book = book6,
			BookId = book6.Id,
			BorrowDate = DateOnly.FromDateTime(DateTime.Today)
		};


		var book7 = books.GetAll(EntityFilter.Active).First(b => b.Title == "The Great Gatsby");
		if (book7.AvailableCopies > 0) book7.AvailableCopies--;
		var loan7 = new Loan
		{
			User = member4,
			UserId = member4.Id,
			Book = book7,
			BookId = book7.Id,
			BorrowDate = today.AddDays(-25),
			ReturnDate = today.AddDays(-5)
		};


		var book8 = books.GetAll(EntityFilter.Active).First(b => b.Title == "Pride and Prejudice");
		if (book8.AvailableCopies > 0) book8.AvailableCopies--;
		var loan8 = new Loan
		{
			User = member9,
			UserId = member9.Id,
			Book = book8,
			BookId = book8.Id,
			BorrowDate = today.AddDays(-35),
			ReturnDate = today.AddDays(-10)
		};


		var book9 = books.GetAll(EntityFilter.Active).First(b => b.Title == "The Hobbit");
		if (book9.AvailableCopies > 0) book9.AvailableCopies--;
		var loan9 = new Loan
		{
			User = member6,
			UserId = member6.Id,
			Book = book9,
			BookId = book9.Id,
			BorrowDate = DateOnly.FromDateTime(DateTime.Today)
		};

		var book10 = books.GetAll(EntityFilter.Active).First(b => b.Title == "Dune");
		if (book10.AvailableCopies > 0) book10.AvailableCopies--;
		var loan10 = new Loan
		{
			User = member7,
			UserId = member7.Id,
			Book = book10,
			BookId = book10.Id,
			BorrowDate = today.AddDays(-15),
			ReturnDate = today.AddDays(-2)
		};


		var book11 = books.GetAll(EntityFilter.Active).First(b => b.Title == "The Alchemist");
		if (book11.AvailableCopies > 0) book11.AvailableCopies--;
		var loan11 = new Loan
		{
			User = member6,
			UserId = member6.Id,
			Book = book11,
			BookId = book11.Id,
			BorrowDate = DateOnly.FromDateTime(DateTime.Today)
		};
		var loan12 = new Loan
		{
			User = member4,
			UserId = member4.Id,
			Book = book11,
			BookId = book11.Id,
			BorrowDate = today.AddDays(-22),
			ReturnDate = today
		};


		var book12 = books.GetAll(EntityFilter.Active).First(b => b.Title == "Sapiens: A Brief History of Humankind");
		if (book12.AvailableCopies > 0) book12.AvailableCopies--;
		var loan13 = new Loan
		{
			User = member9,
			UserId = member9.Id,
			Book = book12,
			BookId = book12.Id,
			BorrowDate = DateOnly.FromDateTime(DateTime.Today)
		};

		var book13 = books.GetAll(EntityFilter.Active).First(b => b.Title == "The Da Vinci Code");
		if (book13.AvailableCopies > 0) book13.AvailableCopies--;
		var loan14 = new Loan
		{
			User = member4,
			UserId = member4.Id,
			Book = book13,
			BookId = book13.Id,
			BorrowDate = today.AddDays(-63),
			ReturnDate = today.AddDays(-5)
		};

		return [loan1, loan2, loan3, loan4, loan5, loan6, loan7, loan8, loan9, loan10, loan11, loan12, loan13, loan14];
	}


	public void Add(Loan loan)
	{
		ArgumentNullException.ThrowIfNull(loan);

		loan.Id = Guid.CreateVersion7();
		loan.BorrowDate = DateOnly.FromDateTime(DateTime.UtcNow);
		loan.DueDate = loan.BorrowDate.AddDays(ValidationConstants.LoanPeriodDays);
		loan.ReturnDate = null;
		loan.Status = LoanStatus.Borrowed;
		loan.IsOverdue = false;
		loan.IsActive = true;
		loan.CreatedAt = DateTime.UtcNow;
		_loans.Add(loan);
	}


	public Loan? GetById(Guid id, LoanFilter filter = LoanFilter.Active)
	{
		return ApplyFilter(_loans, filter).FirstOrDefault(l => l.Id == id);
	}


	public IReadOnlyList<Loan> GetAll(LoanFilter filter = LoanFilter.Active)
	{
		return [.. ApplyFilter(_loans, filter)];
	}


	public IReadOnlyList<Loan> GetByUserId(Guid userId, LoanFilter filter = LoanFilter.Active)
	{
		return [.. ApplyFilter(_loans, filter).Where(l => l.UserId == userId)];
	}


	public IReadOnlyList<Loan> GetByBookId(Guid bookId, LoanFilter filter = LoanFilter.Active)
	{
		return [.. ApplyFilter(_loans, filter).Where(l => l.BookId == bookId)];
	}


	public IReadOnlyList<Loan> GetLoansByBookAndUser(Guid bookId, Guid userId, LoanFilter filter = LoanFilter.All)
	{
		return [.. ApplyFilter(_loans, filter).Where(l => l.BookId == bookId && l.UserId == userId)];
	}


	public void Update(Loan loan)
	{
		// In-memory collections update by reference automatically.
		// However, we leave this method empty rather than throwing an exception 
		// so that the Service layer can safely call _repository.Update() 
		// without crashing, simulating a real database save operation.
	}


	// ---------- Private helper ----------
	private static IEnumerable<Loan> ApplyFilter(IEnumerable<Loan> source, LoanFilter filter)
	{
		return filter switch
		{
			LoanFilter.All => source,
			LoanFilter.Active => source.Where(l => l.IsActive),
			LoanFilter.Inactive => source.Where(l => !l.IsActive),
			LoanFilter.Overdue => source.Where(l => l.IsOverdue),
			LoanFilter.Returned => source.Where(l => l.ReturnDate.HasValue),
			_ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
		};
	}
}