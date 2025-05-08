using Entities;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface ITaskService
    {
        Task<TaskResponse> AddTask(TaskAddRequest? task);
        Task<IEnumerable<TaskResponse>?> GetAllGroupTasks(Guid groupId);
        Task<TaskResponse?> GetTaskById(Guid? groupId, Guid? taskId);
        Task<TaskResponse?> UpdateTask(TaskUpdateRequest? task);
        Task<IEnumerable<TaskResponse>?> GetAllCompletedTask();
        Task<bool> DeleteTask(ToDoItem toDoItem);
        Task<bool> CheckAsCompleted(Guid? taskId);
    }
}
