using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Domain.ValueObjects;

public record BookGenre
{
	public Genre Value { get; }
	private BookGenre(Genre value) => Value = value; 

	public static BookGenre Create(Genre value) {
		if (!Enum.IsDefined(typeof(Genre), value))
			throw new ArgumentException("Genre value is not a recognized genre.", nameof(value));
		return new BookGenre(value);
	}


	public override string ToString()=> Value.ToString();
	public static implicit operator Genre(BookGenre genre) => genre.Value;
}