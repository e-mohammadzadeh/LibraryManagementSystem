using LibraryManagementSystem.Application.DTOs.AuditLog;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Services;

public interface IAuditLogManagementService
{
	void Record(AuditAction action, string entityType, int entityId, string? details = null);
	IReadOnlyList<AuditLogDto> GetAll();
	IReadOnlyList<AuditLogDto> GetByPerformedByUserId(int userId);
	IReadOnlyList<AuditLogDto> GetByEntity(string entityType, int entityId);
}