using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Domain.Persistence.Configurations;

public class FineConfiguration : IEntityTypeConfiguration<Fine>
{
	public void Configure(EntityTypeBuilder<Fine> builder)
	{
		builder.HasKey(f => f.Id);

		builder.HasOne(f => f.Loan)
			.WithMany()
			.HasForeignKey(f => f.LoanId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne<User>()
			.WithMany()
			.HasForeignKey(f => f.UserId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasIndex(f => f.LoanId);
		builder.Property(f => f.OverdueDays).IsRequired();
		builder.Property(f => f.Status).IsRequired();
		builder.Property(f => f.PaidAt);
		builder.Property(f => f.Reason).IsRequired().HasMaxLength(500);
		builder.Property(f => f.Action).IsRequired();
	}
}