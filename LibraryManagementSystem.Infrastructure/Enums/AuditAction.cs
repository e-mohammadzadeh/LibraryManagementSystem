namespace LibraryManagementSystem.Infrastructure.Enums;

public enum AuditAction
{
	UserCreated,
	UserUpdated,
	UserRemoved,
	UserRestored,
	UserPasswordChanged,
	MembershipRenewed,

	AuthorCreated,
	AuthorUpdated,
	AuthorRemoved,
	AuthorRestored,

	TranslatorCreated,
	TranslatorUpdated,
	TranslatorRemoved,
	TranslatorRestored,

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