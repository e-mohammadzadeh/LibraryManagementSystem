using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Domain.Persistence.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
	public void Configure(EntityTypeBuilder<Person> builder)
	{
		builder.ToTable("People");

		builder.HasKey(p => p.Id);
		builder.Property(p => p.Id).ValueGeneratedNever();

		builder.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
		builder.Property(p => p.LastName).IsRequired().HasMaxLength(100);
		builder.Property(p => p.NationalCode).IsRequired().HasMaxLength(20);
		builder.Property(p => p.Email).HasConversion(email => email.Value, value => Email.Create(value)).IsRequired().HasMaxLength(254);
		builder.Property(p => p.PhoneNumber).HasConversion(ph => ph.Value, value => PhoneNumber.Create(value))
			.IsRequired().HasMaxLength(16);
		builder.Property(p => p.BirthDate).IsRequired();
		builder.Property(p => p.CreatedAt).IsRequired();

		builder.HasIndex(p => p.NationalCode).IsUnique();
		builder.HasIndex(p => p.Email).IsUnique();
		builder.HasIndex(p => p.PhoneNumber).IsUnique();

		builder.Property(p => p.UpdatedAt).IsRequired(false);
		builder.Property(p => p.UpdatedByUserId).IsRequired(false);
	}
}