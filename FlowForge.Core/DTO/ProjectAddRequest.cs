using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Enums;

namespace FlowForge.Core.DTO
{
    public class ProjectAddRequest
    {
        public string? ProjectTitle { get; set; }
        public string SelectedColor { get; set; } = "#8b1c32";
        public Guid CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public ProjectVisibility ProjectVisibility { get; set; } = ProjectVisibility.Private;
    
        public Project ToProject()
        {
            return new Project
            {
                ProjectTitle = ProjectTitle,
                SelectedColor = SelectedColor,
                CreatedAt = CreatedAt,
                CreatedById = CreatedById,
                ProjectVisibility = ProjectVisibility
            };
        }
    }
}
