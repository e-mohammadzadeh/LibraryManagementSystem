using LibraryManagementSystem.Domain;
using System.Text.RegularExpressions;
using LibraryManagementSystem.Common;
using LibraryManagementSystem.Helpers;

namespace LibraryManagementSystem.Services;

public class BookManagementService
{
	private List<Book> _listOfBooks = new();

	public ServiceResult<Book> AddBook(string isbn, string bookName, Author author, DateOnly publishDate,
		int totalCopies)
	{
		if (IsbnValidate(isbn))
		{
			if (!IsExistIsbn(isbn, _listOfBooks))
			{
				Book newBook = new Book(isbn, bookName, author, publishDate, totalCopies);
				_listOfBooks.Add(newBook);
				return ServiceResult<Book>.ShowSuccessMessage(newBook);
			}
			else
			{
				return ServiceResult<Book>.ShowFailMessage("This ISBN is already exists. Please enter a new ISBN.");
			}
		}
		else
		{
			return ServiceResult<Book>.ShowFailMessage("Invalid ISBN format. Please enter a valid 10 or 13 digit ISBN.");
		}
	}

	private static bool IsbnValidate(string isbn)
	{
		string cleaned = isbn.Replace("-", "");

		switch (cleaned.Length)
		{
			// Check for ISBN-13 (must be exactly 13 digits)
			case 13 when Regex.IsMatch(cleaned, @"^\d{13}$"):
			// Check for ISBN-10 (must be 10 characters: 9 digits + optional 'X')
			case 10 when Regex.IsMatch(cleaned, @"^\d{9}[\dX]$"):
				return true;
			default:
				return false;
		}
	}

	private static bool IsExistIsbn(string isbn, List<Book> listOfBooks)
	{
		return listOfBooks.Any(book => book.InternationalStandardBookNumber == isbn);
	}
}