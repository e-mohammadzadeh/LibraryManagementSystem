using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Domain.Persistence.Configurations;

public class TranslatorConfiguration : IEntityTypeConfiguration<Translator>
{
	public void Configure(EntityTypeBuilder<Translator> builder) {
		builder.ToTable("Translators");
		builder.HasKey(a => a.Id);
		builder.Property(a => a.Id).ValueGeneratedNever();

		builder.HasOne(a => a.Person)
			.WithOne(p => p.Translator)
			.HasForeignKey<Author>(a => a.Id)
			.OnDelete(DeleteBehavior.Restrict);
	}
}