using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Enums;

namespace FlowForge.Core.DTO;

public class TaskResponse
{
    public Guid TaskId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? ScheduleDateTime { get; set; }
    public bool IsRecurring { get; set; } = false;
    public int RecurringInterval { get; set; } = 0;
    public Guid? SectionId { get; set; }
    public string SectionName { get; set; } = "General";
    public Guid ProjectId { get; set; }
    public ProjectTaskStatus Status { get; set; }
    public override bool Equals(object? obj)
    {
        if (obj is not TaskResponse taskResponse)
            return false;

        return TaskId == taskResponse.TaskId
            && Title == taskResponse.Title
            && Description == taskResponse.Description
            && ScheduleDateTime == taskResponse.ScheduleDateTime
            && IsRecurring == taskResponse.IsRecurring
            && RecurringInterval == taskResponse.RecurringInterval
            && ProjectId == taskResponse.ProjectId
            && Status == taskResponse.Status;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TaskId, Title, Description, Status);
    }

    public override string ToString()
    {
        return $"{TaskId}, {Title}, {Description}, {Status}, {ProjectId}";
    }

    public ProjectTask ToTask()
    {
        return new ProjectTask()
        {
            TaskId = TaskId,
            Title = Title,
            Description = Description,
            ScheduleDateTime = ScheduleDateTime,
            IsRecurring = IsRecurring,
            RecurringInterval = RecurringInterval,
            ProjectId = ProjectId,
            SectionId = SectionId,
            Status = Status
        };
    }
}

public static class TaskExtensions
{
    public static TaskResponse ToTaskResponse(this ProjectTask task)
    {
        return new TaskResponse
        {
            TaskId = task.TaskId,
            Title = task.Title,
            Description = task.Description,
            ScheduleDateTime = task.ScheduleDateTime,
            IsRecurring = task.IsRecurring,
            RecurringInterval = task.RecurringInterval,
            ProjectId = task.ProjectId,
            SectionId = task.SectionId,
            Status = task.Status
        };
    }
}
