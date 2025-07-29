using FlowForge.Core.Domain.IdentityEntities;

namespace FlowForge.Core.Domain.Entities
{
    public class ProjectSection
    {
        public Guid SectionId { get; set; }
        public string? SectionName { get; set; }
        public Guid ProjectId { get; set; }
        public Guid CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public ICollection<ProjectTask> Tasks { get; set; } = [];
    }
}
