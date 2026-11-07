namespace LibraryManagementSystem.Domain.Exceptions;

public class MembershipExpiredException : DomainException
{
	public MembershipExpiredException(string memberName) : base($"Membership of '{memberName}' has expired.") { }
}