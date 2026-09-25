using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Infrastructure.Enums;

namespace LibraryManagementSystem.Infrastructure.DTOs.AuditLog;

public class AuditLogDto
{
	public Guid Id { get; init; }
	public DateTime OccurredAt { get; init; }
	public Guid PerformedByUserId { get; init; }
	public string PerformedByName { get; init; } = null!;
	public AuditAction Action { get; init; }
	public string EntityType { get; init; } = null!;
	public Guid EntityId { get; init; }
	public string? Details { get; init; }
}