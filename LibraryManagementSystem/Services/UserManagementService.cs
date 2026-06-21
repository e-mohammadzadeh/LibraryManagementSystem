using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;

namespace LibraryManagementSystem.Services;

public class UserManagementService
{
	private List<Author> _authors = new();
	private List<LibraryUser> _members = new();
	private List<LibraryUser> _managers = new();


	public ServiceResult<Author> AddAuthor(string firstName, string lastName, string nationalCode, string email,
		string phoneNumber, DateOnly birthDate, string? biography)
	{
		// Check first-last name for duplication author
		// Check national code for duplication author

		var newAuthor = new Author(firstName, lastName, nationalCode, email, phoneNumber, birthDate, biography);
		_authors.Add(newAuthor);

		return ServiceResult<Author>.Ok(newAuthor);
	}

	public List<Author> GetAllAuthors()
	{
		return _authors;
	}

	public Author? FindAuthorById(int? id)
	{
		return _authors.FirstOrDefault(a => a.AuthorId == id);
	}

	public void AddMember()
	{
	}
	// RegisterMember  EditMember  DeactivateMember  FindUserById
}