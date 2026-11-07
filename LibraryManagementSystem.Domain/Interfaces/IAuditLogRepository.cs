using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IAuditLogRepository
{
	void Add(AuditLog auditLog);
	IReadOnlyList<AuditLog> GetAll();
	IReadOnlyList<AuditLog> GetByPerformedByUserId(Guid userId);
	IReadOnlyList<AuditLog> GetByEntity(string entityType, Guid entityId);
}