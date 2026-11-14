using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.DTOs.AuditLog;
using LibraryManagementSystem.Infrastructure.Enums;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class AuditLogManagementService : IAuditLogManagementService
{
	private readonly IUserRepository _userRepository;
	private readonly IAuditLogRepository _auditLogRepository;
	private readonly ICurrentUserSession _currentUserSession;


	public AuditLogManagementService(IUserRepository userRepository, IAuditLogRepository auditLogRepository,
		ICurrentUserSession currentUserSession)
	{
		_userRepository = userRepository;
		_auditLogRepository = auditLogRepository;
		_currentUserSession = currentUserSession;
	}


	public void Record(AuditAction action, string entityType, Guid entityId, string? details = null)
	{
		if (!_currentUserSession.IsAuthenticated)
			throw new InvalidOperationException("An authenticated user is required to record this action.");

		CreateAuditLog(_currentUserSession.CurrentUser.Id, action, entityType, entityId, details);
	}


	public void RecordSystemEvent(AuditAction action, string entityType, Guid entityId, string? details = null)
	{
		CreateAuditLog(null, action, entityType, entityId, details);
	}


	public IReadOnlyList<AuditLogDto> GetAll() { return MapToDto(_auditLogRepository.GetAll()); }


	public IReadOnlyList<AuditLogDto> GetByPerformedByUserId(Guid userId)
	{
		return MapToDto(_auditLogRepository.GetByPerformedByUserId(userId));
	}


	public IReadOnlyList<AuditLogDto> GetByEntity(string entityType, Guid entityId)
	{
		return MapToDto(_auditLogRepository.GetByEntity(entityType, entityId));
	}


	private void CreateAuditLog(Guid? performedByUserId, AuditAction action, string entityType, Guid entityId,
		string? details)
	{
		var auditLog = new AuditLog
		{
			PerformedByUserId = performedByUserId,
			Action = action,
			EntityType = entityType,
			EntityId = entityId,
			Details = details
		};

		_auditLogRepository.Add(auditLog);
	}


	private IReadOnlyList<AuditLogDto> MapToDto(IReadOnlyList<AuditLog> auditLogs)
	{
		return
		[
			.. auditLogs.OrderByDescending(a => a.OccurredAt).Select(auditLog =>
			{
				var user = auditLog.PerformedByUserId.HasValue
					? _userRepository.GetById(auditLog.PerformedByUserId.Value, EntityFilter.All)
					: null;

				var performedByName = user is not null
					? $"{user.FirstName} {user.LastName}"
					: "System";

				return auditLog.ToDto(performedByName);
			})
		];
	}
}