using FlowForge.Core.Domain.Entities;

namespace FlowForge.Core.Domain.RepositoryContract;

public interface ITaskRepository
{
    Task<ProjectTask> CreateTask(ProjectTask task);
    Task<List<ProjectTask>> GetTasks(Guid userId ,Guid ProjectId);
    Task<ProjectTask?> GetTaskById(Guid? ProjectId, Guid? taskId);
    Task<ProjectTask?> UpdateTask(ProjectTask task);
    Task<IEnumerable<ProjectTask>> GetCompletedTasks();
    Task<bool> DeleteTask(ProjectTask task);
    Task<bool> CheckAsCompleted(Guid? taskId);
}