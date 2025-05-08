using Entities;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface IGroupService
    {
        Task<TasksGroup> CreateGroup(GroupAddRequest groupAddRequest);
        Task<List<TasksGroup>> GetGroups();
        Task<GroupResponse> GetGroupById(Guid? groupId);
        Task<GroupResponse> UpdateGroup(Guid? groupId, GroupUpdateRequest groupUpdateRequest);
        Task<bool> DeleteGroup(Guid groupId);
    }
}
