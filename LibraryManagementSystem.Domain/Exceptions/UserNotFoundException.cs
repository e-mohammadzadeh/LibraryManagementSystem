namespace LibraryManagementSystem.Domain.Exceptions;

public class UserNotFoundException : DomainException
{
	public UserNotFoundException(Guid userId) : base($"User with id '{userId}' was not found.") { }
}