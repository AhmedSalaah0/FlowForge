using System.Text.RegularExpressions;
using Entities;
namespace RepositoryContract;
public interface IGroupRepository
{
    Task<TasksGroup> CreateGroup(TasksGroup group);
    Task<List<TasksGroup>> GetGroups();
    Task<TasksGroup?> GetGroupById(Guid? id);
    Task<TasksGroup?> UpdateGroup(Guid? groupId, TasksGroup group);
    Task<bool> DeleteGroup(Guid groupId);
}