namespace LibraryManagementSystem;

public class Admin:Person
{
	public Admin()
	{
		Managers = new List<Manager>();
		Members = new List<Member>();
	} 
	public List<Manager> Managers { get; set; }
	public List<Member> Members { get; set; }
}