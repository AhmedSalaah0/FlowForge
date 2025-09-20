using FlowForge.Core.Domain.Entities;

namespace FlowForge.Core.DTO
{
    public class ProjectAddRequest
    {
        public string? ProjectTitle { get; set; }
        public string SelectedColor { get; set; } = "#8b1c32";
        public Guid CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
    
        public Project ToProject()
        {
            return new Project
            {
                ProjectTitle = ProjectTitle,
                SelectedColor = SelectedColor,
                CreatedAt = CreatedAt,
                CreatedById = CreatedById
            };
        }
    }
}
