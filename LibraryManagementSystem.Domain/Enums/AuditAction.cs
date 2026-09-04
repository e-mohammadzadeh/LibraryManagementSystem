namespace LibraryManagementSystem.Domain.Enums;

public enum AuditAction
{
	UserCreated,
	UserUpdated,
	UserRemoved,
	UserPasswordChanged,
	MembershipRenewed,

	AuthorCreated,
	AuthorUpdated,
	AuthorRemoved,

	TranslatorCreated,
	TranslatorUpdated,
	TranslatorRemoved,

	BookCreated,
	BookUpdated,
	BookRetired,

	LoanBorrowed,
	LoanReturned,
	LoanRenewed,
	LoanMarkedLost,

	FineCreated,
	FinePaid,
	FineWaived,

	UserLoggedIn,
	UserLoggedOut
}