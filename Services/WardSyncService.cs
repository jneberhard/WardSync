using WardSync.Models;

namespace WardSync.Services;

public class WardSyncService
{
    public Task<List<Calling>> GetCallingsAsync()
    {
        var callings = new List<Calling>
        {
            new Calling
            {
                Id = 1,
                Title = "Primary Teacher",
                Organization = "Primary",
                MemberAssigned = "Sample Member",
                Status = "Open",
                NeedsFollowUp = true
            }
        };

        return Task.FromResult(callings);
    }

    public Task<List<AssignmentItem>> GetAssignmentsAsync()
    {
        var assignments = new List<AssignmentItem>
        {
            new AssignmentItem
            {
                Id = 1,
                Title = "Youth Activity Planning",
                Description = "Plan transport and refreshments.",
                AssignedLeader = "Sample Leader",
                Organization = "Young Women",
                DueDate = DateTime.Today.AddDays(7),
                Priority = "High",
                Status = "Open"
            }
        };

        return Task.FromResult(assignments);
    }
}