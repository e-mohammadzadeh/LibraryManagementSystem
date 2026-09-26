using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class AuditLog
{
	public Guid Id { get; set; }
	public DateTime OccurredAt { get; set; }
	public Guid PerformedByUserId { get; set; }
	public AuditAction Action { get; set; }
	public string EntityType { get; set; } = string.Empty;
	public Guid EntityId { get; set; }
	public string? Details { get; set; }
}