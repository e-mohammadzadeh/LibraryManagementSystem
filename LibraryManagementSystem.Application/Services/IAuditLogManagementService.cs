using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Services;

public interface IAuditLogManagementService
{
	void Record(AuditAction action, string entityType, int entityId, string? details = null);
}