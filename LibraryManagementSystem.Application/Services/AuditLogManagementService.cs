using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class AuditLogManagementService
{
	private readonly IAuditLogRepository _auditLogRepository;
	private readonly ICurrentUserSession _currentUserSession;


	public AuditLogManagementService(IAuditLogRepository auditLogRepository, ICurrentUserSession currentUserSession)
	{
		_auditLogRepository = auditLogRepository;
		_currentUserSession = currentUserSession;
	}


	public void Record(AuditAction action, string entityType, int entityId, string? details = null)
	{
		if (!_currentUserSession.IsAuthenticated || !_currentUserSession.UserId.HasValue) return;

		var auditLog = new AuditLog(_currentUserSession.UserId.Value, action, entityType, entityId, details);
		_auditLogRepository.Add(auditLog);
	}
}