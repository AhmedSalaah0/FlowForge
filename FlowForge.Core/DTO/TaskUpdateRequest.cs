using FlowForge.Core.Domain.Entities;

namespace FlowForge.Core.DTO;

public class TaskUpdateRequest
{
        public Guid TaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool Success { get; set; }
        public Guid ProjectId { get; set; }
        public Guid MemberId { get; set; }

    public ProjectTask ToTask()
        {
            return new ProjectTask
            {
                TaskId = TaskId,
                Title = Title,
                Description = Description,
                Success = Success,
                ProjectId = ProjectId,
                MemberId = MemberId
            };
        }
}


