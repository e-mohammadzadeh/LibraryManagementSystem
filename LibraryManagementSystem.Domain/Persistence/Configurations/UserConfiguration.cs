using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Domain.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.HasOne(u => u.Role)
			.WithMany()
			.HasForeignKey(u => u.RoleId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(u => u.IsActive).IsRequired();
		builder.Property(u => u.MembershipStartDate).IsRequired();
		builder.Property(u => u.MembershipExpiryDate).IsRequired();
		builder.Property(u => u.ShouldRemove).IsRequired();
	}
}