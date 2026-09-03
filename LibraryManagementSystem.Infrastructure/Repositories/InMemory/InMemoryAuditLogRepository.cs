using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Infrastructure.Repositories.InMemory;

public class InMemoryAuditLogRepository : IAuditLogRepository
{
	private readonly List<AuditLog> _auditLogs = [];
	public IReadOnlyList<AuditLog> GetAll() { return _auditLogs; }


	public IReadOnlyList<AuditLog> GetByUserId(int userId)
	{
		return [.. _auditLogs.Where(a => a.EntityType == "User" && a.EntityId == userId)];
	}


	public IReadOnlyList<AuditLog> GetByPerformedByUserId(int userId)
	{
		return [.. _auditLogs.Where(a => a.PerformedByUserId == userId)];
	}


	public void Add(AuditLog auditLog) { _auditLogs.Add(auditLog); }
}