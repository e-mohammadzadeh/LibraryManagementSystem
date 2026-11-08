using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Domain.Persistence.Configurations;

public class TranslatorConfiguration : IEntityTypeConfiguration<Translator>
{
	public void Configure(EntityTypeBuilder<Translator> builder)
	{
		builder
			.HasMany(t => t.BookTranslators)
			.WithOne(bt => bt.Translator)
			.HasForeignKey(bt => bt.TranslatorId);
	}
}