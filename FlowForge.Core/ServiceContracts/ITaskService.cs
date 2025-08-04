using FlowForge.Core.Domain.Entities;
using FlowForge.Core.DTO;

namespace FlowForge.Core.ServiceContracts
{
    public interface ITaskService
    {
        Task<TaskResponse> AddTask(Guid userId, TaskAddRequest? task);
        Task<IEnumerable<SectionWithTasksResponse>?> GetAllProjectTasks(Guid userId, Guid projectId);
        Task<TaskResponse?> GetTaskById(Guid? projectId, Guid? taskId);
        Task<TaskResponse?> UpdateTask(TaskUpdateRequest? task);
        Task<IEnumerable<TaskResponse>?> GetAllCompletedTask(Guid userId);
        Task<bool> DeleteTask(ProjectTask task);
        Task<bool> UpdateTaskStatus(TaskUpdateStatus taskUpdateStatus);
        Task<bool> ScheduleTask(ScheduleTaskRequest request);
        Task<bool> UnScheduleTask(Guid taskId, Guid projectId, Guid userId);
        Task MoveTask(Guid userId, MoveTaskRequest request);
    }
}
