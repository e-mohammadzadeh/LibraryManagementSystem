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
		if (_authors.Any(author => author.FirstName == firstName && author.LastName == lastName))
			return ServiceResult<Author>.Fail("An author with the same first and last name already exists.");


		if (_authors.Any(author => author.NationalCode == nationalCode))
			return ServiceResult<Author>.Fail("An author with the same national code already exists.");

		var newAuthor = new Author(firstName, lastName, nationalCode, email, phoneNumber, birthDate, biography);
		_authors.Add(newAuthor);

		return ServiceResult<Author>.Ok(newAuthor, "Author added successfully.");
	}

	public IReadOnlyList<Author> GetAllAuthors()
	{
		return _authors;
	}

	public Author? FindAuthorById(int id)
	{
		return _authors.FirstOrDefault(a => a.AuthorId == id);
	}

	public ServiceResult<Author> UpdateAuthor(int authorId, string? firstName, string? lastName, string? nationalCode,
		string? email, string? phoneNumber, DateOnly? birthDate, string? biography)
	{
		var author = FindAuthorById(authorId);
		if (author == null)
			return ServiceResult<Author>.Fail("Problem occurred while updating the author.");

		if (_authors.Any(aut => aut.AuthorId != authorId && aut.FirstName == firstName))
			return ServiceResult<Author>.Fail("An author with the same first name already exists.");

		author.FirstName = firstName ?? author.FirstName;

		if (_authors.Any(aut => aut.AuthorId != authorId && aut.LastName == lastName))
			return ServiceResult<Author>.Fail("An author with the same last name already exists.");

		author.LastName = lastName ?? author.LastName;

		if (_authors.Any(aut => aut.AuthorId != authorId && aut.NationalCode == nationalCode))
			return ServiceResult<Author>.Fail("An author with the same national code already exists.");

		author.NationalCode = nationalCode ?? author.NationalCode;
		author.Email = email ?? author.Email;
		author.PhoneNumber = phoneNumber ?? author.PhoneNumber;
		author.BirthDate = birthDate ?? author.BirthDate;
		author.Biography = biography ?? author.Biography;
		return ServiceResult<Author>.Ok(author, "Author updated successfully.");
	}

	public ServiceResult<Author> RemoveAuthor(Author author)
	{
		// TODO	After implementing Loan class and service, before deleting author should check that none of books isn't borrowed
		if (author.Books.Count != 0)
			return ServiceResult<Author>.Fail("Failed to remove author. The author has associated books.");

		_authors.Remove(author);
		return ServiceResult<Author>.Ok(author, "Author removed successfully.");
	}

	public void AddMember()
	{
	}
	// RegisterMember  EditMember  DeactivateMember  FindUserById
}