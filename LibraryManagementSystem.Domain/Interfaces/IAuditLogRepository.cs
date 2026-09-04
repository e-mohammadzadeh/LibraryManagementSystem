using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IAuditLogRepository
{
	IReadOnlyList<AuditLog> GetAll();
	IReadOnlyList<AuditLog> GetByPerformedByUserId(int userId);
	IReadOnlyList<AuditLog> GetByEntity(string entityType, int entityId);
	void Add(AuditLog auditLog);
}