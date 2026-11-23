using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Domain.Persistence;

namespace LibraryManagementSystem.Application.Repositories.EFCore;

public class UserRepository : PersonRoleRepository<User>, IUserRepository
{
	public UserRepository(LibraryDbContext context) : base(context) { }


	public void Update(User user)
	{
		ArgumentNullException.ThrowIfNull(user);

		var tracked = GetTrackedById(user.Id);

		tracked.RoleId = user.RoleId;
		tracked.IsActive = user.IsActive;
		tracked.MembershipStartDate = user.MembershipStartDate;
		tracked.MembershipExpiryDate = user.MembershipExpiryDate;
		tracked.ShouldRemove = user.ShouldRemove;

		SaveRoleUpdate(tracked, user);
	}


	protected override Exception CreateNotFoundException(Guid id) => new UserNotFoundException(id);
}