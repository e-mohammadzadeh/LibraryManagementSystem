using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Domain.Persistence.Configurations;

public class BookAuthorConfiguration :IEntityTypeConfiguration<BookAuthor>
{
	public void Configure(EntityTypeBuilder<BookAuthor> builder)
	{
		builder.HasKey(ba => ba.Id);

		builder.HasOne(ba => ba.Book)
			.WithMany()
			.HasForeignKey(ba => ba.BookId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(ba => ba.Author)
			.WithMany(a => a.BookAuthors)
			.HasForeignKey(ba => ba.AuthorId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}