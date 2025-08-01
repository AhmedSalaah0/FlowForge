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

    public async Task<List<ProjectSection>> GetTasks(Guid userId, Guid groupId)
    {
        return await _context.Sections
            .Where(section => section.ProjectId == groupId)
            .Include(section => section.Tasks)
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
        task.SectionId = taskToUpdate.SectionId;
        _context.Entry(taskToUpdate).CurrentValues.SetValues(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> MoveTask(ProjectTask task, Guid NewSectionId)
    {
        task.SectionId = NewSectionId;
        int num = await _context.SaveChangesAsync();
        return num > 0;
    }
}