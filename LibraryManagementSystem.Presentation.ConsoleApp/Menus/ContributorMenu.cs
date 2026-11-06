using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Menus;

public abstract class ContributorMenu<TService> where TService : Contributor
{
	protected readonly TService Service;

	public ContributorMenu(TService service)
	{
		Service = service;
	}


}