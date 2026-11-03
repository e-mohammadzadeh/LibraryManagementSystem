using LibraryManagementSystem.Infrastructure.DTOs;

namespace LibraryManagementSystem.Infrastructure.Interfaces;

public interface IMenuService
{
	IReadOnlyList<MenuOptionDto> GetChildren(string id);
}