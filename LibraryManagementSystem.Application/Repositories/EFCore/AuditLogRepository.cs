using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class AuditLogRepository : IAuditLogRepository
{
	protected readonly LibraryDbContext Context;


	public AuditLogRepository(LibraryDbContext context) { Context = context; }


	public void Add(AuditLog auditLog)
	{
		ArgumentNullException.ThrowIfNull(auditLog);

		auditLog.Id = Guid.CreateVersion7();
		auditLog.OccurredAt = DateTime.UtcNow;

		Context.AuditLogs.Add(auditLog);
		Context.SaveChanges();
		Context.Entry(auditLog).State = EntityState.Detached;
	}


	public IReadOnlyList<AuditLog> GetAll() =>
		[.. Context.AuditLogs.AsNoTracking().OrderByDescending(a => a.OccurredAt)];


	public IReadOnlyList<AuditLog> GetByPerformedByUserId(Guid userId) =>
	[
		.. Context.AuditLogs
			.AsNoTracking()
			.Where(a => a.PerformedByUserId == userId)
			.OrderByDescending(a => a.OccurredAt)
	];


	public IReadOnlyList<AuditLog> GetByEntity(string entityType, Guid entityId) =>
	[
		.. Context.AuditLogs
			.AsNoTracking()
			.Where(a => a.EntityType == entityType && a.EntityId == entityId)
			.OrderByDescending(a => a.OccurredAt)
	];
}