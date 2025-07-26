namespace FlowForge.Core.Domain.Entities
{
    public class ProjectTask
    {
        public Guid TaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool Success { get; set; } = false;
        public Guid ProjectId { get; set; }
        public Guid MemberId { get; set; }
        public DateTime? ScheduledDateTime { get; set; }
        public bool IsRecurring { get; set; } = false;
        public int RecurringInterval { get; set; } = 0;
    }
}
