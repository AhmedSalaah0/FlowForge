using Entities;

namespace RepositoryContract;

public interface ITaskRepository
{
    Task<ToDoItem> CreateTask(ToDoItem task);
    Task<List<ToDoItem>> GetTasks(Guid groupId);
    Task<ToDoItem?> GetTaskById(Guid? groupId, Guid? taskId);
    Task<ToDoItem?> UpdateTask(ToDoItem task);
    Task<IEnumerable<ToDoItem>> GetCompletedTasks();
    Task<bool> DeleteTask(ToDoItem task);
    Task<bool> CheckAsCompleted(Guid? taskId);
}