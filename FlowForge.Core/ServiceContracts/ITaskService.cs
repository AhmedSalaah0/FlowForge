using FlowForge.Core.Domain.Entities;
using FlowForge.Core.DTO;

namespace FlowForge.Core.ServiceContracts
{
    public interface ITaskService
    {
        Task<TaskResponse> AddTask(Guid userId, TaskAddRequest? task);
        Task<IEnumerable<SectionWithTasksResponse>?> GetAllProjectTasks(Guid userId, Guid projectId);
        Task<TaskResponse?> GetTaskById(Guid? projectId, Guid? taskId, bool track = true);
        Task<TaskResponse?> UpdateTask(TaskUpdateRequest? task);
        Task<IEnumerable<TaskResponse>?> GetAllCompletedTask(Guid userId);
        Task<bool> DeleteTask(Guid userId, Guid projectId, Guid taskId);
        Task<bool> UpdateTaskStatus(TaskUpdateStatus taskUpdateStatus);
        Task<bool> ScheduleTask(ScheduleTaskRequest request);
        Task<bool> UnScheduleTask(Guid taskId, Guid projectId, Guid userId);
        Task<TaskResponse> AssignTask(AssignTaskRequest assignTask); 
        Task MoveTask(Guid userId, MoveTaskRequest request);
        Task<bool> ReorderTask(ReorderRequest reorderRequest, Guid userId);
    }
}
