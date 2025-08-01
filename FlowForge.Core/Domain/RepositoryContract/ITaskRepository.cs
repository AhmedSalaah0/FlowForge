using FlowForge.Core.Domain.Entities;
using FlowForge.Core.DTO;

namespace FlowForge.Core.Domain.RepositoryContract;

public interface ITaskRepository
{
    Task<ProjectTask> CreateTask(ProjectTask task);
    Task<List<ProjectSection>> GetTasks(Guid userId ,Guid ProjectId);
    Task<ProjectTask?> GetTaskById(Guid? ProjectId, Guid? taskId);
    Task<ProjectTask?> UpdateTask(ProjectTask task);
    Task<IEnumerable<ProjectTask>> GetCompletedTasks();
    Task<bool> DeleteTask(ProjectTask task);
    Task<bool> CheckAsCompleted(Guid? taskId);
    Task<bool> MoveTask(ProjectTask task, Guid NewSectionId);
}