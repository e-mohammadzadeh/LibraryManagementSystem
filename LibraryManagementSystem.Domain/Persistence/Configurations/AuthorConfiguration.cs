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

		builder.HasIndex(a => a.PersonId).IsUnique();
	}
}