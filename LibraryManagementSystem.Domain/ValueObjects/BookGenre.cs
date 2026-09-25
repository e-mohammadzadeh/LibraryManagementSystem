using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.ValueObjects;

public record BookGenre
{
	public Genre Value { get; }

}