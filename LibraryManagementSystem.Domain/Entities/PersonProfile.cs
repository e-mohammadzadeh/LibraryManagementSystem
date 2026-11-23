namespace LibraryManagementSystem.Domain.Entities;

public abstract class PersonProfile
{
	public Guid Id { get; set; }
	public Person Person { get; set; } = null!;
	public bool IsRemoved { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public Guid? UpdatedByUserId { get; set; }
}