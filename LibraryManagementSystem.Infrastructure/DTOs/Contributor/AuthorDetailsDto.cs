using LibraryManagementSystem.Infrastructure.DTOs.Books;

namespace LibraryManagementSystem.Infrastructure.DTOs.Contributor;

public record AuthorDetailsDto(ContributorDto Author, IReadOnlyList<BookDto> Books);
