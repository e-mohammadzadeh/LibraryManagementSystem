using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class UserAutoRemovalService
{
	private readonly IUserRepository _userRepository;
	private readonly ILoanRepository _loanRepository;
	private readonly IFineRepository _fineRepository;


	public UserAutoRemovalService(IUserRepository userRepository, ILoanRepository loanRepository,
		IFineRepository fineRepository)
	{
		_userRepository = userRepository;
		_loanRepository = loanRepository;
		_fineRepository = fineRepository;
	}


	public bool CanBeAutoRemoved(int userId)
	{
		var user = _userRepository.FindById(userId);
		if (user is null || !user.ShouldRemove) return false;
		if (_loanRepository.GetActiveLoansByUser(userId).Count > 0) return false;
		if (_fineRepository.HasUnpaidFines(userId)) return false;
		return true;
	}


	public ServiceResult<string> TryAutoRemove(int userId)
	{
		if (!CanBeAutoRemoved(userId)) return ServiceResult<string>.Fail("User does not meet auto-removal conditions.");

		var user = _userRepository.FindById(userId)!;
		_userRepository.Remove(user);
		return ServiceResult<string>.Ok($"{user.FirstName} {user.LastName}",
			"User automatically removed from the system.");
	}
}