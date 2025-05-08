using Entities;
using RepositoryContract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Repositories;
public class GroupRepository(ApplicationDbContext context, ILogger<GroupRepository> logger) : IGroupRepository
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<GroupRepository> _logger = logger;
    public async Task<TasksGroup> CreateGroup(TasksGroup group)
    {
        await _context.AddAsync(group);
        await _context.SaveChangesAsync();
        return group; 
    }

    public async Task<TasksGroup?> GetGroupById(Guid? id)
    {
        TasksGroup? group = await _context.groups.FirstOrDefaultAsync(g => g.GroupId == id);
        return group;
    }

    public async Task<List<TasksGroup>> GetGroups()
    {
        return await _context.groups.OrderBy(g => g.createdAt).ToListAsync();
    }

    public async Task<TasksGroup?> UpdateGroup(Guid? groupId, TasksGroup group)
    {
        var groupToUpdate = _context.groups.FirstOrDefault(g => g.GroupId == groupId);
        if(groupToUpdate == null)
            return null;
        group.createdAt = groupToUpdate.createdAt;
        _context.Entry(groupToUpdate).CurrentValues.SetValues(group);
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task<bool> DeleteGroup(Guid groupId)
    {
        _context.RemoveRange(_context.groups.Where(g => g.GroupId == groupId));
        await _context.SaveChangesAsync();
        return true;
    }
}