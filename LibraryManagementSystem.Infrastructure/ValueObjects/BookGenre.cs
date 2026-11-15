using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Infrastructure.ValueObjects;

public record BookGenre
{
	public Genre Value { get; }
	private BookGenre(Genre value) => Value = value;


	public static BookGenre Create(Genre value)
	{
		return !Enum.IsDefined(value)
			? throw new ArgumentException("Genre value is not a recognized genre.", nameof(value))
			: new BookGenre(value);
	}


	public override string ToString() => Value.ToString();
	public static implicit operator Genre(BookGenre genre) => genre.Value;
}