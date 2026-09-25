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
	BookRemoved,

	LoanBorrowed,
	LoanReturned,
	LoanRenewed,

	FineCreated,
	FinePaid,
	FineWaived,

	UserLoggedIn,
	UserLoggedOut
}