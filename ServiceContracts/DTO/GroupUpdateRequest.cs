using System.Dynamic;
using Entities;
namespace ServiceContracts.DTO
{
    public class GroupUpdateRequest
    {
        public Guid groupId { get; set; }
        public string? groupTitle { get; set; }
        public string selectedColor { get; set; }
        public TasksGroup ToGroupTasks()
        {
            return new TasksGroup
            {
                GroupId = groupId,
                groupTitle = groupTitle,
                selectedColor = selectedColor,
            };
        }
    }
}
