using FlowForge.Core.Domain.IdentityEntities;
using FlowForge.Core.Enums;

namespace FlowForge.Core.Domain.Entities
{
    public class Project
    {
        public Guid ProjectId { get; set; }
        public Guid CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public string? ProjectTitle { get; set; }
        public string SelectedColor { get; set; }
        public DateTime CreatedAt { get; set; }
        public ProjectVisibility ProjectVisibility { get; set; }
        public ICollection<ProjectMember> ProjectMembers { get; set; } = [];
        public ICollection<ProjectSection> Sections { get; set; } = [];

    }
}
