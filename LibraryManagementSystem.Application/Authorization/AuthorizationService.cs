using LibraryManagementSystem.Application.Authentication;
using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.Authorization;

public class AuthorizationService : IAuthorizationService
{
	private readonly ICurrentUserSession _session;


	public AuthorizationService(ICurrentUserSession session) { _session = session; }


	public bool HasPermission(Permission permission)
	{
		if (!_session.IsAuthenticated || _session.CurrentUser is null) return false;

		return _session.CurrentUser.Roles.SelectMany(RolePermissionMap.GetPermissions).Contains(permission);
	}


	public bool HasAnyPermission(params Permission[] permissions) => permissions.Any(HasPermission);
}