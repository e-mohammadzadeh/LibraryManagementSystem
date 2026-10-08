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
		if (!_currentUserSession.IsAuthenticated || !_currentUserSession.UserId.HasValue) return;

		var auditLog = new AuditLog
		{
			PerformedByUserId = _currentUserSession.UserId.Value,
			Action = action,
			EntityType = entityType,
			EntityId = entityId,
			Details = details
		};
		_auditLogRepository.Add(auditLog);
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


	private IReadOnlyList<AuditLogDto> MapToDto(IReadOnlyList<AuditLog> auditLogs)
	{
		return
		[
			.. auditLogs.OrderByDescending(a => a.OccurredAt).Select(auditLog =>
			{
				var user = _userRepository.FindById(auditLog.PerformedByUserId, EntityFilter.All);

				var performedByName = user is not null
					? $"{user.FirstName} {user.LastName}"
					: "Unknown User";

				return auditLog.ToDto(performedByName);
			})
		];
	}
}