using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagementSystem.Domain.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
	public void Configure(EntityTypeBuilder<AuditLog> builder)
	{
		builder.HasKey(a => a.Id);
		builder.Property(a => a.PerformedByUserId).IsRequired();
		builder.Property(a => a.OccurredAt).IsRequired();
		builder.Property(a => a.Action).IsRequired();
		builder.Property(a => a.EntityType).IsRequired().HasMaxLength(100);
		builder.Property(a => a.Details).HasMaxLength(2000);
		builder.HasIndex(a => a.OccurredAt);
		builder.HasIndex(a => a.PerformedByUserId);
		builder.HasIndex(a => new
		{
			a.EntityType,
			a.EntityId
		});
	}
}