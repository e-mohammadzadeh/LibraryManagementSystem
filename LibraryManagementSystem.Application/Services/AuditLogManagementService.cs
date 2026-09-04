using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Application.DTOs.AuditLog;
using LibraryManagementSystem.Application.Mapping;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;

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


	public void Record(AuditAction action, string entityType, int entityId, string? details = null)
	{
		if (!_currentUserSession.IsAuthenticated || !_currentUserSession.UserId.HasValue) return;

		var auditLog = new AuditLog(_currentUserSession.UserId.Value, action, entityType, entityId, details);
		_auditLogRepository.Add(auditLog);
	}


	public IReadOnlyList<AuditLogDto> GetAll() { return MapToDto(_auditLogRepository.GetAll()); }


	public IReadOnlyList<AuditLogDto> GetByPerformedByUserId(int userId)
	{
		return MapToDto(_auditLogRepository.GetByPerformedByUserId(userId));
	}


	public IReadOnlyList<AuditLogDto> GetByEntity(string entityType, int entityId)
	{
		return MapToDto(_auditLogRepository.GetByEntity(entityType, entityId));
	}


	private IReadOnlyList<AuditLogDto> MapToDto(IReadOnlyList<AuditLog> auditLogs)
	{
		return
		[
			.. auditLogs.OrderByDescending(a => a.OccurredAt).Select(auditLog =>
			{
				var user = _userRepository.FindById(
					auditLog.PerformedByUserId);

				var performedByName = user is not null
					? $"{user.FirstName} {user.LastName}"
					: "Unknown User";

				return auditLog.ToDto(performedByName);
			})
		];
	}
}