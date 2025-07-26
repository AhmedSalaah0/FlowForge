using FlowForge.Core.Domain.Entities;

namespace FlowForge.Core.DTO;

public class TaskResponse
{
    public Guid TaskId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool Success { get; set; } = false;
    public DateTime? ScheduleDateTime { get; set; }
    public bool IsRecurring { get; set; } = false;
    public int RecurringInterval { get; set; } = 0;
    public Guid ProjectId { get; set; }
    public override bool Equals(object? obj)
    {
        if (obj is not TaskResponse taskResponse)
            return false;

        return TaskId == taskResponse.TaskId
            && Title == taskResponse.Title
            && Description == taskResponse.Description
            && Success == taskResponse.Success
            && ScheduleDateTime == taskResponse.ScheduleDateTime
            && IsRecurring == taskResponse.IsRecurring
            && RecurringInterval == taskResponse.RecurringInterval
            && ProjectId == taskResponse.ProjectId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TaskId, Title, Description, Success);
    }

    public override string ToString()
    {
        return $"{TaskId}, {Title}, {Description}, {Success}, {ProjectId}";
    }

    public ProjectTask ToToDoItem()
    {
        return new ProjectTask()
        {
            TaskId = TaskId,
            Title = Title,
            Description = Description,
            ScheduledDateTime = ScheduleDateTime,
            IsRecurring = IsRecurring,
            RecurringInterval = RecurringInterval,
            Success = Success,
            ProjectId = ProjectId,
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
            Success = task.Success,
            ScheduleDateTime = task.ScheduledDateTime,
            IsRecurring = task.IsRecurring,
            RecurringInterval = task.RecurringInterval,
            ProjectId = task.ProjectId,
        };
    }
}
