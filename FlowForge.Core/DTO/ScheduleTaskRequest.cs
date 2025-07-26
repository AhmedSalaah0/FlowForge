using FlowForge.Core.Domain.Entities;

namespace FlowForge.Core.DTO;

public class ScheduleTaskRequest
{
    public Guid? TaskId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid MemberId { get; set; }
    public DateOnly ScheduleDate { get; set; }
    public TimeOnly ScheduleTime { get; set; }
    public bool IsRecurring { get; set; }
    public int RecurringInterval { get; set; } = 1;
}

public static class ScheduleTaskRequestExtensions
{
    public static ProjectTask ToTask(this ScheduleTaskRequest request)
    {
        return new ProjectTask
        {
            TaskId = request.TaskId ?? Guid.NewGuid(),
            ProjectId = request.ProjectId ?? Guid.Empty,
            MemberId = request.MemberId,
            ScheduledDateTime = new DateTime(request.ScheduleDate.Year, request.ScheduleDate.Month, request.ScheduleDate.Day, request.ScheduleTime.Hour, request.ScheduleTime.Minute, 0),
            IsRecurring = request.IsRecurring,
            RecurringInterval = request.RecurringInterval
        };
    }
}
