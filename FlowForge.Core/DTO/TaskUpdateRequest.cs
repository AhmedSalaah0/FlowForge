using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Enums;

namespace FlowForge.Core.DTO;

public class TaskUpdateRequest
{
        public Guid TaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool Success { get; set; }
        public Guid ProjectId { get; set; }
        public Guid MemberId { get; set; }
        public ProjectTaskStatus Status { get; set; }

    public ProjectTask ToTask()
        {
            return new ProjectTask
            {
                TaskId = TaskId,
                Title = Title,
                Description = Description,
                ProjectId = ProjectId,
                CreatedById = MemberId,
                Status = Status
            };
        }
}


