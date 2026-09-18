using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IAuditLogRepository
{
	IReadOnlyList<AuditLog> GetAll();
	IReadOnlyList<AuditLog> GetByPerformedByUserId(Guid userId);
	IReadOnlyList<AuditLog> GetByEntity(string entityType, Guid entityId);
	void Add(AuditLog auditLog);
}