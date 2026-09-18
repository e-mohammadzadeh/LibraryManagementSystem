using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.DTOs.AuditLog;

namespace LibraryManagementSystem.Application.Mapping;

public static class AudiLogMapper
{
	public static AuditLogDto ToDto(this AuditLog auditLog, string performedByName)
	{
		return new AuditLogDto
		{
			Id = auditLog.Id,
			Action = auditLog.Action,
			EntityType = auditLog.EntityType,
			EntityId = auditLog.EntityId,
			OccurredAt = auditLog.OccurredAt,
			PerformedByUserId = auditLog.PerformedByUserId,
			PerformedByName = performedByName,
			Details = auditLog.Details
		};
	}
}