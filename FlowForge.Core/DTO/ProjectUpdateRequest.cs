using FlowForge.Core.Domain.Entities;

namespace FlowForge.Core.DTO
{
    public class ProjectUpdateRequest
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public string? ProjectTitle { get; set; }
        public string SelectedColor { get; set; }
        public Project ToProject()
        {
            return new Project
            {
                ProjectId = ProjectId,
                CreatedById = UserId,
                ProjectTitle = ProjectTitle,
                SelectedColor = SelectedColor,
            };
        }
    }
}
