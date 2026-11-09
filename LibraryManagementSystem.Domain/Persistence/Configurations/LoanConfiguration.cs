using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Domain.Persistence.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
	public void Configure(EntityTypeBuilder<Loan> builder)
	{
		builder.HasKey(l => l.Id);

		builder.HasOne(l => l.Book)
			.WithMany()
			.HasForeignKey(l => l.BookId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne(l => l.User)
			.WithMany()
			.HasForeignKey(l => l.UserId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Property(l => l.BorrowDate).IsRequired();
		builder.Property(l => l.DueDate).IsRequired();
		builder.Property(l => l.Status).IsRequired();
		builder.Property(l => l.Action).IsRequired();
		builder.Property(l => l.RenewalCount).IsRequired();
		builder.Property(l => l.IsOverdue).IsRequired();
		builder.Property(l => l.IsActive).IsRequired();
	}
}