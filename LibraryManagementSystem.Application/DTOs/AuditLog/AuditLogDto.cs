using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.DTOs.AuditLog;

public class AuditLogDto
{
	public int Id { get; init; }
	public DateTime OccurredAt { get; init; }
	public int PerformedByUserId { get; init; }
	public string PerformedByName { get; init; } = null!;
	public AuditAction Action { get; init; }
	public string EntityType { get; init; } = null!;
	public int EntityId { get; init; }
	public string? Details { get; init; }
}