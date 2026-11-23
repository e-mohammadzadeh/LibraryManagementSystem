using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Domain.Persistence.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
	public void Configure(EntityTypeBuilder<Author> builder)
	{
		builder.ToTable("Authors");
		builder.HasKey(a => a.Id);
		builder.Property(a => a.Id).ValueGeneratedNever();

		builder.HasOne(a => a.Person)
			.WithOne(p => p.Author)
			.HasForeignKey<Author>(a => a.PersonId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Property(a => a.PersonId).IsRequired();
		builder.HasIndex(a => a.PersonId).IsUnique();

		builder.Property(a => a.DeathDate).IsRequired(false);
		builder.Property(a => a.Biography).HasMaxLength(2000).IsRequired(false);

		builder.Property(a => a.IsRemoved).IsRequired();
		builder.Property(a => a.CreatedAt).IsRequired();

		builder.Property(a => a.UpdatedAt).IsRequired(false);
		builder.Property(a => a.UpdatedByUserId).IsRequired(false);
	}
}