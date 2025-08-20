using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Enums;

namespace FlowForge.Core.DTO
{
    public class TaskAddRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        public Guid SectionId { get; set; }
        public string Status { get; set; } = "Pending";
        public int? Order { get; set; }

        public ProjectTask ToTask()
        {
            return new ProjectTask()
            {
                Title = Title,
                Description = Description,
                ProjectId = ProjectId,
                SectionId = SectionId,
                Status = Enum.Parse<ProjectTaskStatus>(Status),
            };
        }
    }
}
