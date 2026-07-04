using System.Xml;
using LibraryManagementSystem.Common;
using LibraryManagementSystem.Domain;
using LibraryManagementSystem.DTOs;

namespace LibraryManagementSystem.Services;

public class UserManagementService
{
	private List<Author> _authors = new();
	private List<Member> _members = new();
	//private readonly List<Manager> _managers = new();


	public ServiceResult<Author> AddAuthor(CreateAuthorDto dto)
	{
		if (_authors.Any(author => author.NationalCode == dto.NationalCode))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByNationalCode);

		if (_authors.Any(author => author.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByEmail);

		var existingSameName = _authors.FirstOrDefault(a =>
			a.FirstName.Equals(dto.FirstName, StringComparison.OrdinalIgnoreCase) &&
			a.LastName.Equals(dto.LastName, StringComparison.OrdinalIgnoreCase));

		if (existingSameName is not null)
			return ServiceResult<Author>.Warning(
				$"An author with the same name already exists (ID: {existingSameName.Id}). ");

		var newAuthor = new Author(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber,
			dto.BirthDate, dto.Biography);

		_authors.Add(newAuthor);
		return ServiceResult<Author>.Ok(newAuthor, ValidationMessages.AuthorAddedSuccessfully);
	}


	public IReadOnlyList<Author> GetAllAuthors()
	{
		return _authors.AsReadOnly();
	}


	public Author? FindAuthorById(int id)
	{
		return _authors.FirstOrDefault(a => a.Id == id);
	}


	public ServiceResult<Author> UpdateAuthor(int authorId, UpdateAuthorDto dto)
	{
		var author = FindAuthorById(authorId);
		if (author is null)
			return ServiceResult<Author>.Fail(ValidationMessages.AuthorUpdateFailed);

		if (IsNoOpUpdateAuthor(author, dto))
			return ServiceResult<Author>.Fail(ValidationMessages.NoChangesDetected);

		var resolvedFirstName = dto.FirstName ?? author.FirstName;
		var resolvedLastName = dto.LastName ?? author.LastName;
		if (dto.FirstName is not null || dto.LastName is not null)
		{
			if (_authors.Any(aut =>
				    aut.Id != authorId && aut.FirstName == resolvedFirstName && aut.LastName == resolvedLastName))
				return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByName);
		}

		if (dto.NationalCode is not null &&
		    _authors.Any(aut => aut.Id != authorId && aut.NationalCode == dto.NationalCode))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByNationalCode);

		if (dto.Email is not null && _authors.Any(aut =>
			    aut.Id != authorId && aut.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByEmail);

		if (dto.PhoneNumber is not null &&
		    _authors.Any(aut => aut.Id != authorId && aut.PhoneNumber == dto.PhoneNumber))
			return ServiceResult<Author>.Fail(ValidationMessages.FailureDuplicateAuthorByPhoneNumber);

		author.Update(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate,
			dto.Biography);

		return ServiceResult<Author>.Ok(author, ValidationMessages.AuthorUpdatedSuccessfully);
	}


	private static bool IsNoOpUpdateAuthor(Author author, UpdateAuthorDto dto)
	{
		return (dto.FirstName == null || dto.FirstName == author.FirstName) &&
		       (dto.LastName == null || dto.LastName == author.LastName) &&
		       (dto.NationalCode == null || dto.NationalCode == author.NationalCode) &&
		       (dto.Email == null || dto.Email == author.Email) &&
		       (dto.PhoneNumber == null || dto.PhoneNumber == author.PhoneNumber) &&
		       (dto.BirthDate == null || dto.BirthDate == author.BirthDate) &&
		       (dto.Biography == null || dto.Biography == author.Biography);
	}


	public ServiceResult<Author> RemoveAuthor(int authorId)
	{
		var author = FindAuthorById(authorId);
		if (author is null)
			return ServiceResult<Author>.Fail(ValidationMessages.AuthorRemoveFailed);

		// TODO	After implementing Loan class and service, before deleting author should check that none of books isn't borrowed
		if (author.Books.Count != 0)
			return ServiceResult<Author>.Fail("Failed to remove author. The author has associated books.");

		_authors.Remove(author);
		return ServiceResult<Author>.Ok(author, ValidationMessages.AuthorRemovedSuccessfully);
	}


	public IReadOnlyList<Author> SearchAuthors(string searchItem, Func<Author, string?> selector)
	{
		if (string.IsNullOrWhiteSpace(searchItem))
			return new List<Author>();

		return _authors.Where(author =>
		{
			var value = selector(author);
			return value is not null && value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
		}).ToList();
	}


	public ServiceResult<Member> AddMember(CreateMemberDto dto)
	{
		if (_members.Any(member => member.NationalCode == dto.NationalCode))
			return ServiceResult<Member>.Fail(ValidationMessages.FailureDuplicateMemberByNationalCode);

		if (_members.Any(member => member.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
			return ServiceResult<Member>.Fail(ValidationMessages.FailureDuplicateMemberByEmail);

		var existingSameName = _members.FirstOrDefault(member =>
			member.FirstName.Equals(dto.FirstName, StringComparison.OrdinalIgnoreCase) &&
			member.LastName.Equals(dto.LastName, StringComparison.OrdinalIgnoreCase));

		if (existingSameName is not null)
			return ServiceResult<Member>.Warning(
				$"A member with the same name already exists (ID: {existingSameName.Id}). ");

		var newMember = new Member(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email,
			dto.PhoneNumber, dto.BirthDate);

		_members.Add(newMember);
		return ServiceResult<Member>.Ok(newMember, ValidationMessages.MemberAddedSuccessfully);
	}


	public IReadOnlyList<Member> GetAllMembers()
	{
		return _members.AsReadOnly();
	}


	public ServiceResult<Member> UpdateMember(int memberId, UpdateMemberDto dto)
	{
		var member = FindMemberById(memberId);
		if (member is null)
			return ServiceResult<Member>.Fail(ValidationMessages.MemberUpdateFailed);

		if (IsNoOpUpdateMember(member, dto))
			return ServiceResult<Member>.Fail(ValidationMessages.NoChangesDetected);

		var resolvedFirstName = dto.FirstName ?? member.FirstName;
		var resolvedLastName = dto.LastName ?? member.LastName;
		if (dto.FirstName is not null || dto.LastName is not null)
		{
			if (_members.Any(member =>
				    member.Id != memberId && member.FirstName == resolvedFirstName &&
				    member.LastName == resolvedLastName))
				return ServiceResult<Member>.Fail(ValidationMessages.FailureDuplicateMemberByName);
		}

		if (dto.NationalCode is not null &&
		    _members.Any(member => member.Id != memberId && member.NationalCode == dto.NationalCode))
			return ServiceResult<Member>.Fail(ValidationMessages.FailureDuplicateMemberByNationalCode);

		if (dto.Email is not null && _members.Any(member =>
			    member.Id != memberId && member.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
			return ServiceResult<Member>.Fail(ValidationMessages.FailureDuplicateMemberByEmail);

		if (dto.PhoneNumber is not null &&
		    _members.Any(member => member.Id != memberId && member.PhoneNumber == dto.PhoneNumber))
			return ServiceResult<Member>.Fail(ValidationMessages.FailureDuplicateMemberByPhoneNumber);

		member.Update(dto.FirstName, dto.LastName, dto.NationalCode, dto.Email, dto.PhoneNumber, dto.BirthDate);
		return ServiceResult<Member>.Ok(member, ValidationMessages.MemberUpdatedSuccessfully);
	}



	public Member? FindMemberById(int id)
	{
		return _members.FirstOrDefault(m => m.Id == id);
	}


	private static bool IsNoOpUpdateMember(Member member, UpdateMemberDto dto)
	{
		return (dto.FirstName == null || dto.FirstName == member.FirstName) &&
		       (dto.LastName == null || dto.LastName == member.LastName) &&
		       (dto.NationalCode == null || dto.NationalCode == member.NationalCode) &&
		       (dto.Email == null || dto.Email == member.Email) &&
		       (dto.PhoneNumber == null || dto.PhoneNumber == member.PhoneNumber) &&
		       (dto.BirthDate == null || dto.BirthDate == member.BirthDate);
	}


	public ServiceResult<Member> RemoveMember(int memberId)
	{
		var member = FindMemberById(memberId);
		if (member is null)
			return ServiceResult<Member>.Fail(ValidationMessages.MemberRemoveFailed);

		// TODO	After implementing Loan class and service, before deleting member should check that none of books isn't borrowed
		//if (member.Books.Count != 0)
		//	return ServiceResult<Member>.Fail("Failed to remove author. The author has associated books.");

		_members.Remove(member);
		return ServiceResult<Member>.Ok(member, ValidationMessages.MemberRemovedSuccessfully);
	}


	public IReadOnlyList<Member> SearchMember(string searchItem, Func<Member, string?> selector)
	{
		if (string.IsNullOrWhiteSpace(searchItem))
			return new List<Member>();

		return _members.Where(member =>
		{
			var value = selector(member);
			return value is not null && value.Contains(searchItem, StringComparison.OrdinalIgnoreCase);
		}).ToList();
	}


	// DeactivateMember  FindUserById
}