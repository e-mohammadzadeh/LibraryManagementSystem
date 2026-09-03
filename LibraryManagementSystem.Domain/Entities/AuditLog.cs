using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Domain.Entities;

public class AuditLog
{
	public AuditLog(int performedByUserId, AuditAction action, string entityType, int entityId, string? details = null)
	{
		Id = ++_nextId;
		OccurredAt = DateTime.Now;
		PerformedByUserId = performedByUserId;
		Action = action;
		EntityType = entityType;
		EntityId = entityId;
		Details = details;
	}


	private static int _nextId;
	public int Id { get; private set; }
	public DateTime OccurredAt { get; private set; }
	public int PerformedByUserId { get; private set; }
	public AuditAction Action { get; private set; }
	public string EntityType { get; private set; }
	public int EntityId { get; private set; }
	public string? Details { get; private set; }
}