using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Domain.Persistence.Configurations;

public class TranslatorConfiguration : IEntityTypeConfiguration<Translator>
{
	public void Configure(EntityTypeBuilder<Translator> builder)
	{
		builder.ToTable("Translators");
		builder.HasKey(a => a.Id);
		builder.Property(a => a.Id).ValueGeneratedNever();

		builder.HasOne(t => t.Person)
			.WithOne(p => p.Translator)
			.HasForeignKey<Translator>(t => t.PersonId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Property(t => t.PersonId).IsRequired();
		builder.HasIndex(t => t.PersonId).IsUnique();

		builder.Property(t => t.DeathDate).IsRequired(false);
		builder.Property(t => t.Biography).HasMaxLength(2000).IsRequired(false);

		builder.Property(t => t.IsRemoved).IsRequired();
		builder.Property(t => t.CreatedAt).IsRequired();

		builder.Property(t => t.UpdatedAt).IsRequired(false);
		builder.Property(t => t.UpdatedByUserId).IsRequired(false);
	}
}