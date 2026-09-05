using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.DTOs.AuditLog;
using LibraryManagementSystem.Presentation.ConsoleApp.Helpers;

namespace LibraryManagementSystem.Presentation.ConsoleApp.Printers;

public class AuditLogPrinter
{
	public static void PrintTable(IReadOnlyList<AuditLogDto> auditLogs, string title = "Audit Log List")
	{
		if (auditLogs.Count == 0)
		{
			ConsoleHelper.ShowWarning(Messages.NotAvailableAuditLog);
			return;
		}

		var headers = new[] { "ID", "Date & Time", "Performed By", "User ID", "Action", "Entity", "Entity ID", "Details" };
		var rows = auditLogs.Select(log =>
			{
				var details = string.IsNullOrWhiteSpace(log.Details)
					? ["—"]
					: ConsoleTable.WrapText(log.Details, 40);

				return new[]
				{
					[log.Id.ToString()],
					[log.OccurredAt.ToString("yyyy-MM-dd HH:mm")],
					ConsoleTable.WrapText(log.PerformedByName, 25),
					[log.PerformedByUserId.ToString()],
					ConsoleTable.WrapText(log.Action.ToString(), 20),
					[log.EntityType],
					[log.EntityId.ToString()],
					details
				};
			})
			.ToList();

		ConsoleTable.PrintTable(title, headers, rows);
	}
}