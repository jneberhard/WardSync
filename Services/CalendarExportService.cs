using System.Text;
using WardSync.Models;

namespace WardSync.Services;

public class CalendarExportService
{
    public string CreateAssignmentCalendarFile(AssignmentItem assignment)
    {
        var startDate = assignment.DueDate.Date;
        var endDate = startDate.AddDays(1);

        var title = EscapeText($"WardSync Assignment: {assignment.Title}");
        var description = EscapeText(
            $"Description: {assignment.Description}\\n" +
            $"Assigned Leader: {assignment.AssignedLeader}\\n" +
            $"Organization: {assignment.Organization}\\n" +
            $"Priority: {assignment.Priority}\\n" +
            $"Status: {assignment.Status}\\n" +
            $"Notes: {assignment.Notes}"
        );

        var builder = new StringBuilder();

        builder.AppendLine("BEGIN:VCALENDAR");
        builder.AppendLine("VERSION:2.0");
        builder.AppendLine("PRODID:-//WardSync//Assignment Calendar//EN");
        builder.AppendLine("BEGIN:VEVENT");
        builder.AppendLine($"UID:wardsync-assignment-{assignment.Id}@wardsync");
        builder.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");
        builder.AppendLine($"DTSTART;VALUE=DATE:{startDate:yyyyMMdd}");
        builder.AppendLine($"DTEND;VALUE=DATE:{endDate:yyyyMMdd}");
        builder.AppendLine($"SUMMARY:{title}");
        builder.AppendLine($"DESCRIPTION:{description}");
        builder.AppendLine("END:VEVENT");
        builder.AppendLine("END:VCALENDAR");

        return builder.ToString();
    }

    private static string EscapeText(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace(",", "\\,")
            .Replace(";", "\\;")
            .Replace("\n", "\\n")
            .Replace("\r", "");
    }
}