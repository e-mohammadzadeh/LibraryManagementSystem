namespace LibraryManagementSystem.Domain.Exceptions;

public class FineNotFoundException : DomainException
{
	public FineNotFoundException(Guid fineId) : base($"Fine with id '{fineId}' was not found.") { }

}