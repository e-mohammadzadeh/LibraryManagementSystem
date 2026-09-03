using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IAuditLogRepository
{
	IReadOnlyList<AuditLog> GetAll();
	IReadOnlyList<AuditLog> GetByUserId(int userId);
	IReadOnlyList<AuditLog> GetByPerformedByUserId(int userId);
	void Add(AuditLog auditLog);
}