using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace FlowForge.Infrastructure.Repositories;
public class TaskRepository(ApplicationDbContext context) : ITaskRepository
{
    private readonly ApplicationDbContext _context = context;
    public async Task<ProjectTask> CreateTask(ProjectTask task)
    {
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<IEnumerable<ProjectTask>> GetCompletedTasks()
    {
        return await _context.Tasks.Where(temp => temp.Success).ToListAsync();
    }

    public Task<ProjectTask?> GetTaskById(Guid groupId, Guid taskId)
    {
        return _context.Tasks.FirstOrDefaultAsync(temp => temp.TaskId == taskId && temp.ProjectId == groupId);
    }

    public Task<ProjectTask?> GetTaskById(Guid? groupId, Guid? taskId)
    {
        return _context.Tasks.FirstOrDefaultAsync(temp => temp.TaskId == taskId && temp.ProjectId == groupId);
    }

    public async Task<List<ProjectTask>> GetTasks(Guid userId, Guid groupId)
    {
        return await _context.Tasks
            .Where(temp => temp.ProjectId == groupId && (temp.MemberId == userId || 
            _context.ProjectMembers.Any(t => t.ProjectId == groupId && t.MemberId == userId)))
            .ToListAsync();
    }

    public async Task<bool> DeleteTask(ProjectTask task)
    {
        _context.RemoveRange(_context.Tasks.Where(temp => temp.TaskId == task.TaskId));
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CheckAsCompleted(Guid? taskId)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(temp => temp.TaskId == taskId);
        if (task is null)
        {
            return false;
        }
        task.Success = !task.Success;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ProjectTask?> UpdateTask(ProjectTask task)
    {
        var taskToUpdate = _context.Tasks.FirstOrDefault(temp => temp.TaskId == task.TaskId);
        if (taskToUpdate is null)
        {
            return null;
        }
        _context.Entry(taskToUpdate).CurrentValues.SetValues(task);
        await _context.SaveChangesAsync();
        return task;
    }
}