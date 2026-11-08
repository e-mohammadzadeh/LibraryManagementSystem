using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Domain.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
	public void Configure(EntityTypeBuilder<Role> builder)
	{
		builder.HasKey(r => r.Id);

		builder.Property(r => r.Name).IsRequired();
		builder.HasIndex(r => r.Name).IsUnique();

		builder.HasData(
			new Role
			{
				Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
				Name = LibraryUserRole.Admin
			},
			new Role
			{
				Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
				Name = LibraryUserRole.Librarian
			},
			new Role
			{
				Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
				Name = LibraryUserRole.Member
			}
		);
	}
}