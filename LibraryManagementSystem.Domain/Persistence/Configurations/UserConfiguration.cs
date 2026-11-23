using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Domain.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("Users");
		builder.HasKey(u => u.Id);
		builder.Property(u => u.Id).ValueGeneratedNever();

		builder.HasOne(u => u.Person)
			.WithOne(p => p.User)
			.HasForeignKey<User>(u => u.PersonId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Property(u => u.PersonId).IsRequired();
		builder.HasIndex(u => u.PersonId).IsUnique();

		builder.HasOne(u => u.Role)
			.WithMany()
			.HasForeignKey(u => u.RoleId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Property(u => u.IsRemoved).IsRequired();
		builder.Property(u => u.IsActive).IsRequired();
		builder.Property(u => u.MembershipStartDate).IsRequired();
		builder.Property(u => u.MembershipExpiryDate).IsRequired();
		builder.Property(u => u.ShouldRemove).IsRequired();

		builder.Property(u => u.CreatedAt).IsRequired();

		builder.Property(u => u.UpdatedAt).IsRequired(false);
		builder.Property(u=> u.UpdatedByUserId).IsRequired(false);
	}
}