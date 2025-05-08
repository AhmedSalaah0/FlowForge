using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContract;

namespace Repositories;
public class TaskRepository(ApplicationDbContext context) : ITaskRepository
{
    private readonly ApplicationDbContext _context = context;
    public async Task<ToDoItem> CreateTask(ToDoItem task)
    {
        await _context.tasks.AddAsync(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<IEnumerable<ToDoItem>> GetCompletedTasks()
    {
        return await _context.tasks.Where(temp => temp.Success == true).ToListAsync();
    }

    public Task<ToDoItem?> GetTaskById(Guid groupId, Guid taskId)
    {
        return _context.tasks.FirstOrDefaultAsync(temp => temp.ItemId == taskId && temp.GroupId == groupId);
    }

    public Task<ToDoItem?> GetTaskById(Guid? groupId, Guid? taskId)
    {
        return _context.tasks.FirstOrDefaultAsync(temp => temp.ItemId == taskId && temp.GroupId == groupId);
    }

    public async Task<List<ToDoItem>> GetTasks(Guid groupId)
    {
        return await _context.tasks.Where(temp => temp.GroupId == groupId).ToListAsync();
    }

    public async Task<bool> DeleteTask(ToDoItem task)
    {
        _context.RemoveRange(_context.tasks.Where(temp => temp.ItemId == task.ItemId));
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CheckAsCompleted(Guid? taskId)
    {
        var task = await _context.tasks.FirstOrDefaultAsync(temp => temp.ItemId == taskId);
        if (task is null)
        {
            return false;
        }
        task.Success = !task.Success;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ToDoItem?> UpdateTask(ToDoItem task)
    {
        var taskToUpdate = _context.tasks.FirstOrDefault(temp => temp.ItemId == task.ItemId);
        if (taskToUpdate is null)
        {
            return null;
        }
        _context.Entry(taskToUpdate).CurrentValues.SetValues(task);
        await _context.SaveChangesAsync();
        return task;
    }
}