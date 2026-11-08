using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Domain.Persistence.Configurations;

public class BookTranslatorConfiguration : IEntityTypeConfiguration<BookTranslator>
{
	public void Configure(EntityTypeBuilder<BookTranslator> builder)
	{
		builder.HasKey(bt => new
		{
			bt.BookId,
			bt.TranslatorId
		});

		builder.HasOne(bt => bt.Book)
			.WithMany()
			.HasForeignKey(bt => bt.BookId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(bt => bt.Translator)
			.WithMany(t => t.BookTranslators)
			.HasForeignKey(bt => bt.TranslatorId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}