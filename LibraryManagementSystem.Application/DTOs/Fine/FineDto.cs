using LibraryManagementSystem.Domain.Enums;

namespace LibraryManagementSystem.Application.DTOs.Fine;

public class FineDto
{
	public int FineId { get; init; }
	public int LoanId { get; init; }
	public int UserId { get; init; }
	public string UserFullName { get; init; } = null!;
	public string BookName { get; init; } = null!;
	public decimal DailyRate { get; init; }
	public int OverdueDays { get; init; }
	public decimal Amount { get; init; }
	public DateOnly IssuedDate { get; init; }
	public FineStatus Status { get; init; }
	public string Reason { get; init; } = null!;
	public DateTime CreatedAt { get; init; }
	public DateOnly? PaidAt { get;init; }
}