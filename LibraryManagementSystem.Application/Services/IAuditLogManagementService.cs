using LibraryManagementSystem.Infrastructure.DTOs.AuditLog;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Application.Services;

public interface IAuditLogManagementService
{
	void Record(AuditAction action, string entityType, Guid entityId, string? details = null);
	IReadOnlyList<AuditLogDto> GetAll();
	IReadOnlyList<AuditLogDto> GetByPerformedByUserId(int userId);
	IReadOnlyList<AuditLogDto> GetByEntity(string entityType, int entityId);
}