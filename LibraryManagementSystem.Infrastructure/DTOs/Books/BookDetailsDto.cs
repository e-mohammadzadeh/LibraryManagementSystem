using LibraryManagementSystem.Infrastructure.DTOs.Contributor;

namespace LibraryManagementSystem.Infrastructure.DTOs.Books;

public class BookDetailsDto
{
	public BookDto Book { get; init; } = null!;

	public IReadOnlyList<ContributorDto> Authors { get; init; } = [];
	public IReadOnlyList<ContributorDto> Translators { get; init; } = [];
}