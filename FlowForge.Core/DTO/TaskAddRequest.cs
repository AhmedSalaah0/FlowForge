using FlowForge.Core.Domain.Entities;

namespace FlowForge.Core.DTO
{
    public class TaskAddRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool Success { get; set; } = false;
        public Guid ProjectId { get; set; }
        public Guid SectionId { get; set; }
        public ProjectTask ToTask()
        {
            return new ProjectTask()
            {
                Title = Title,
                Description = Description,
                Success = Success,
                ProjectId = ProjectId,
                SectionId = SectionId
            };
        }
    }
}
