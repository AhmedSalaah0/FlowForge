using Entities;

namespace ServiceContracts.DTO
{
    public class TaskAddRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool Success { get; set; } = false;
        public Guid GroupId { get; set; }


        public ToDoItem ToTask()
        {
            return new ToDoItem()
            {
                Title = this.Title,
                Description = this.Description,
                Success = this.Success,
                GroupId = this.GroupId
            };
        }
    }
}
