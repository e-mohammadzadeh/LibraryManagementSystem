namespace LibraryManagementSystem.Domain.Entities;

public class UserRole
{
	public UserRole(User user, Role role)
	{
		User = user ?? throw new ArgumentNullException(nameof(user));
		Role = role ?? throw new ArgumentNullException(nameof(role));

		UserId = user.Id;
		RoleId = role.Id;
	}


	public Guid UserId { get; }
	public User User { get; }
	public Guid RoleId { get; }
	public Role Role { get; }
}