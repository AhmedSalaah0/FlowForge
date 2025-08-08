using FlowForge.Core.Domain.IdentityEntities;
using FlowForge.Core.Enums;

namespace FlowForge.Core.Domain.Entities
{
    public class ProjectTask
    {
        public Guid TaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }
        public Guid? SectionId { get; set; }
        public ProjectSection? Section { get; set; }
        public Guid CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public DateTime? ScheduleDateTime { get; set; }
        public bool IsRecurring { get; set; } = false;
        public int RecurringInterval { get; set; } = 0;
        public ProjectTaskStatus Status { get; set; } = ProjectTaskStatus.Pending;
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public Guid? AssigneeId { get; set; }
        public ProjectMember? Assignee { get; set; }
    }
}
