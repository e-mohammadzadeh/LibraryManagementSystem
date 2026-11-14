using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Domain.Persistence.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
	public void Configure(EntityTypeBuilder<Book> builder)
	{
		builder.HasKey(b => b.Id);

		builder.Property(b => b.Title).IsRequired().HasMaxLength(300);
		builder.Property(b => b.ISBN).HasConversion(isbn => isbn.Value, value => ISBN.Create(value)).IsRequired()
			.HasMaxLength(13);
		builder.HasIndex(b => b.ISBN).IsUnique();
		builder.Property(b => b.PublishDate).IsRequired();
		builder.Property(b => b.Genre).HasConversion(genre => genre.Value, value => BookGenre.Create(value))
			.IsRequired();
		builder.Property(b => b.Publisher).IsRequired().HasMaxLength(200);
		builder.Property(b => b.OriginalLanguage).IsRequired();
		builder.Property(b => b.Description).HasMaxLength(2000);
		builder.Property(b => b.TotalCopies).IsRequired();
		builder.Property(b => b.AvailableCopies).IsRequired();
	}
}