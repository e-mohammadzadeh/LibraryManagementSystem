namespace LibraryManagementSystem.Application.Services;

public class AuthenticationService
{
	public void Login(string email, byte[] password)
	{
		Login(email, password)
			↓
		UserRepository.FindByEmail(email)
			↓
		User not found ?
			├── Yes → Login failed
			│
			└── No
			↓
		Verify password
			↓

		Password valid?
			├── No → Login failed
			│
			└── Yes
			↓
		Create Session
			↓

		Login successful
	}


	public void Logout()
	{
		Logout()
			↓
		Clear CurrentUserSession
			↓
		Return to Login Menu
	}


	public void GetCurrentUser() { }

	public void IsAuthenticated(){}

}