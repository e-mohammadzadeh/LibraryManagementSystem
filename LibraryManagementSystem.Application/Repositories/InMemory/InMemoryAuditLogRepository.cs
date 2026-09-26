namespace LibraryManagementSystem.Application.Repositories.InMemory;

public class InMemoryAuditLogRepository : IAuditLogRepository
{
	private readonly List<AuditLog> _auditLogs = [];
	public IReadOnlyList<AuditLog> GetAll() { return _auditLogs; }


	public IReadOnlyList<AuditLog> GetByPerformedByUserId(Guid userId)
	{
		return [.. _auditLogs.Where(a => a.PerformedByUserId == userId)];
	}


	public IReadOnlyList<AuditLog> GetByEntity(string entityType, Guid entityId)
	{
		return [.. _auditLogs.Where(a => a.EntityType == entityType && a.EntityId == entityId)];
	}


	public void Add(AuditLog auditLog) { _auditLogs.Add(auditLog); }
}