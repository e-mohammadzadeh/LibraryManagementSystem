using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class LoanManagementService
{
	private readonly ILoanRepository _loanRepository;
	private readonly IUserRepository _userRepository;
	private readonly IBookRepository _bookRepository;


	public LoanManagementService(ILoanRepository loanRepository, IUserRepository userRepository,
		IBookRepository bookRepository)
	{
		_loanRepository = loanRepository;
		_userRepository = userRepository;
		_bookRepository = bookRepository;
	}


	public ServiceResult<Loan> BorrowBook(int userId, int bookId)
	{
		var user = _userRepository.FindById(userId);
		if (user is null)
			return ServiceResult<Loan>.Fail(ValidationMessages.NotUserMatched);

		if (!user.IsActive)
			return ServiceResult<Loan>.Fail("Membership of this user expired. Please first renew membership.");

		var book = _bookRepository.FindById(bookId);
		if (book is null)
			return ServiceResult<Loan>.Fail(ValidationMessages.NotBookMatched);

	

		bookId.
		// check book availability
		if (desiredBook.AvailableCopies <= 0)
		{
			// No copies available
			return;
		}

		// checked LibraryManagementSystem rules
		if (Active Loans > 5)
		{
			// cannot borrow another
			return;
		}

		// check duplicate borrowing
		if (bookId is in viewBorrowedBooks())
		{
			// you are not allowed to borrow repetitive copy of this book
			return;
		}
		// BorrowDate = today
		// DueDate = today + 14 days
		// returnDate = null
		// Status = Borrowed

		// Update Book
		bookId.borrowcCopy();

		// save loan
		LoandRepository.Add();

		// show message
		Console.WriteLine("Book borrowed successfully.  \t\t\tBorrow Date:\r\n\t\t17 July");
		Console.WriteLine("\t\t\tDue Date:\r\n\t\t31 July");
	}


	public void ReturnBook()
	{
	}


	public void GetBorrowedBooks()
	{
	}


	public void GetLoandByUser()
	{
	}


	public void GetLoanByBook()
	{
	}


	public bool CanBorrow()
	{
		return true;
	}


	public bool CanReturn()
	{
		return true;
	}
}