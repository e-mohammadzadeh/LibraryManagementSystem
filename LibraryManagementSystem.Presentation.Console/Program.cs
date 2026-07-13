namespace LibraryManagementSystem;

public static class Program
{
	private static void Main()
	{
		List<int> grades = new() { 1, 2, 3, 4, 5 };
		
		var userManagementService = new UserManagementService();
		var bookManagementService = new BookManagementService(userManagementService);
		MainMenu.MainMenuController(userManagementService, bookManagementService);
	}
}