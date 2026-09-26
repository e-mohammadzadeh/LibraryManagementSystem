using System.ComponentModel.DataAnnotations;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Infrastructure.Common;

namespace LibraryManagementSystem.Domain.Rules;

public class LoanRenewalRules
{
	public static bool CanRenew(Loan loan, out string errorMessage)
	{
		if (loan.ReturnDate.HasValue)
		{
			errorMessage = "Returned books cannot be renewed.";
			return false;
		}

		if (loan.IsOverdue)
		{
			errorMessage = "Overdue loans cannot be renewed. Please return the book and pay any applicable fine.";
			return false;
		}

		if (loan.RenewalCount >= loan.MaxRenewals)
		{
			ValidationConstants.MinRenewMembershipYear
			errorMessage = "This loan has already reached the maximum number of renewals.";
			return false;
		}

		errorMessage = string.Empty;
		return true;
	}
}