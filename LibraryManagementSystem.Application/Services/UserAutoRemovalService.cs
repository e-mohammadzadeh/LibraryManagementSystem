using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Interfaces;
using LibraryManagementSystem.Infrastructure.Enums.Filters;
using LibraryManagementSystem.Infrastructure.Interfaces;

namespace LibraryManagementSystem.Application.Services;

public class UserAutoRemovalService : IUserAutoRemovalService
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


	private bool CanBeAutoRemoved(User? user)
	{
		if (user is null) return false;
		if (!user.ShouldRemove) return false;
		if (_loanRepository.GetAllByUser(user.Id, LoanFilter.Active).Count > 0) return false;
		return !_fineRepository.HasFines(user.Id, FineFilter.Unpaid);
	}


	public ServiceResult<string> TryAutoRemove(Guid userId)
	{
		var user = _userRepository.FindById(userId, EntityFilter.Active)!;
		if (!CanBeAutoRemoved(user)) return ServiceResult<string>.Fail(Messages.UserAutoRemoveNotEligible);
		_userRepository.Remove(user);
		return ServiceResult<string>.Ok($"{user.FirstName} {user.LastName}", Messages.UserAutoRemovedSuccessfully);
	}
}