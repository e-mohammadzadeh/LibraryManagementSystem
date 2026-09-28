namespace LibraryManagementSystem.Domain.Exceptions;

public class LoanNotFoundException : DomainException
{
	public LoanNotFoundException(Guid loanId) : base($"Loan with Id {loanId} not found.") { }
}