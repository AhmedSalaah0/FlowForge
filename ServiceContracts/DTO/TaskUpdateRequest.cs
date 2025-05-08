using Entities;

namespace ServiceContracts.DTO;

public class TaskUpdateRequest
{
        public Guid TaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool Success { get; set; }
        public Guid GroupId { get; set; }

        public ToDoItem ToTask()
        {
            return new ToDoItem
            {
                ItemId = TaskId,
                Title = Title,
                Description = Description,
                Success = Success,
                GroupId = GroupId
            };
        }
}


