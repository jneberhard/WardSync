namespace WardSync.Models;

public class Calling
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Organization { get; set; } = "";
    public string MemberAssigned { get; set; } = "";
    public string Status { get; set; } = "Open";
    public bool NeedsFollowUp { get; set; }
}