using ServiceContracts;
using Entities;
using ServiceContracts.DTO;
using RepositoryContract;

namespace Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _context;
        public GroupService(IGroupRepository context)
        {
            _context = context;
        }
        public async Task<TasksGroup> CreateGroup(GroupAddRequest groupAddRequest)
        {
            ArgumentNullException.ThrowIfNull(groupAddRequest);
            groupAddRequest.createdAt = DateTime.Now;
            TasksGroup groupTasks = groupAddRequest.ToGroupTasks();
            var group = await _context.CreateGroup(groupTasks);
            return group;
        }

        public async Task<List<TasksGroup>> GetGroups()
        {
            List<TasksGroup>? groups = await _context.GetGroups();
            return groups;
        }

        public async Task<GroupResponse> GetGroupById(Guid? groupId)
        {
            if (groupId == null)
            {
                throw new ArgumentNullException(nameof(groupId));
            }

            var group = await _context.GetGroupById(groupId);
            if (group == null)
            {
                throw new ArgumentException(nameof(group));
            }

            return group.ToGroupResponse();
        }
        public async Task<GroupResponse> UpdateGroup(Guid? groupId, GroupUpdateRequest groupUpdateRequest)
        {
            var group = await _context.GetGroupById(groupId);
            if (group == null)
            {
                throw new ArgumentException(nameof(group)); 
            }
            
            await _context.UpdateGroup(group.GroupId , groupUpdateRequest.ToGroupTasks());
            return group.ToGroupResponse();
        }
        public async Task<bool> DeleteGroup(Guid groupId)
        {
            if (groupId == Guid.Empty) return false;


            var group = await _context.GetGroupById(groupId);
            if (group == null) return false;
            bool result = await _context.DeleteGroup(groupId);
            return result;
        }
    }
}
