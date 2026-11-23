using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Domain.Interfaces;

public interface IPersonRoleEntity
{
	Guid Id { get; set; }
	Guid PersonId { get; set; }
	Person Person { get; set; }

	bool IsRemoved { get; set; }

	DateTime CreatedAt { get; set; }
	DateTime? UpdatedAt { get; set; }
	Guid? UpdatedByUserId { get; set; }
}