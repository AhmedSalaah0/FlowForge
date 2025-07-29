using FlowForge.Core.Domain.IdentityEntities;

namespace FlowForge.Core.Domain.Entities
{
    public class ProjectTask
    {
        public Guid TaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool Success { get; set; } = false;
        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }
        public Guid? SectionId { get; set; }
        public ProjectSection? Section { get; set; }
        public Guid CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public DateTime? ScheduleDateTime { get; set; }
        public bool IsRecurring { get; set; } = false;
        public int RecurringInterval { get; set; } = 0;
    }
}
