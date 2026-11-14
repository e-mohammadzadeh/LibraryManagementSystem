using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Domain.Persistence.Configurations;

public class ContributorConfiguration : IEntityTypeConfiguration<Contributor>
{
	public void Configure(EntityTypeBuilder<Contributor> builder)
	{
		builder.Property(c => c.Biography).IsRequired(false).HasMaxLength(2000);
		builder.Property(c => c.DeathDate).IsRequired(false);
	}
}