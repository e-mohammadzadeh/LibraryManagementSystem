using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class AuditLog
{
	public AuditLog(Guid performedByUserId, AuditAction action, string entityType, Guid entityId, string? details = null)
	{
		Id = Guid.CreateVersion7();
		OccurredAt = DateTime.UtcNow;
		PerformedByUserId = performedByUserId;
		Action = action;
		EntityType = entityType;
		EntityId = entityId;
		Details = details;
	}


	public Guid Id { get; private set; }
	public DateTime OccurredAt { get; private set; }
	public Guid PerformedByUserId { get; private set; }
	public AuditAction Action { get; private set; }
	public string EntityType { get; private set; }
	public Guid EntityId { get; private set; }
	public string? Details { get; private set; }
}