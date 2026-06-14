namespace LibraryManagementSystem;

public class Manager:Person
{
	public Manager()
	{
		Members = new List<Member>();
	}
	public List<Member> Members { get; set; }
}