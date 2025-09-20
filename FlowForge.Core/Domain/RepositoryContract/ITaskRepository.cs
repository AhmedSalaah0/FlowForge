using FlowForge.Core.Domain.Entities;
using FlowForge.Core.DTO;
using FlowForge.Core.Enums;
using Microsoft.EntityFrameworkCore.Storage;

namespace FlowForge.Core.Domain.RepositoryContract;

public interface ITaskRepository
{
    Task<ProjectTask> CreateTask(ProjectTask task);
    Task<List<ProjectSection>> GetTasks(Guid userId ,Guid ProjectId);
    Task<ProjectTask?> GetTaskById(Guid? ProjectId, Guid? taskId, bool track = true);
    Task<ProjectTask?> UpdateTask(ProjectTask task);
    Task<IEnumerable<ProjectTask>> GetCompletedTasks();
    Task<bool> DeleteTask(ProjectTask task);
    Task<bool> UpdateTaskStatus(Guid? taskId, ProjectTaskStatus status);
    Task<bool> MoveTask(ProjectTask task, Guid NewSectionId);
    Task<ProjectTask> AssignTask(ProjectTask task, Guid MemberId);
    void UpdateTaskOrder(ProjectTask task);

    Task SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
}