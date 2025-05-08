using Entities;

namespace ServiceContracts.DTO
{
    public class TaskResponse
    {
        public Guid TaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool Success { get; set; } = false;
        public Guid GroupId { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj is not TaskResponse taskResponse)
                return false;

            return TaskId == taskResponse.TaskId
                && Title == taskResponse.Title
                && Description == taskResponse.Description
                && Success == taskResponse.Success && 
                GroupId == taskResponse.GroupId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TaskId, Title, Description, Success);
        }

        public override string ToString()
        {
            return $"{this.TaskId}, {this.Title}, {this.Description}, {this.Success}, {this.GroupId}";
        }

        public ToDoItem ToToDoItem()
        {
            return new ToDoItem()
            {
                ItemId = this.TaskId,
                Title = this.Title,
                Description = this.Description,
                Success = this.Success,
                GroupId = this.GroupId,
            };
        }
    }

    public static class TaskExtensions
    {
        public static TaskResponse ToTaskResponse(this ToDoItem task)
        {
            return new TaskResponse
            {
                TaskId = task.ItemId,
                Title = task.Title,
                Description = task.Description,
                Success = task.Success,
                GroupId = task.GroupId,
            };
        }
    }
}
