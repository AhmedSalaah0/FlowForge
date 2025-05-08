using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContract;
namespace Services
{
    public class TaskService(ITaskRepository context, IGroupService groupService) : ITaskService
    {
        private readonly ITaskRepository _context = context;
        private readonly IGroupService _groupService = groupService;

        public async Task<TaskResponse> AddTask(TaskAddRequest? task)
        {
            ArgumentNullException.ThrowIfNull(task);

            var toDoTask = task.ToTask();
            toDoTask.ItemId = Guid.NewGuid();
            await _context.CreateTask(toDoTask);
            var taskResponse = toDoTask.ToTaskResponse();
            return taskResponse;
        }

        public async Task<IEnumerable<TaskResponse>?> GetAllGroupTasks(Guid groupId)
        {
            if (groupId == Guid.Empty)
            {
                return null;
            }
            var Tasks = await _context.GetTasks(groupId);
            if (Tasks == null)
            {
                return null;
            }
            return Tasks.Select(t => t.ToTaskResponse()); // Return the list of tasks beloning to the groupId no
        }

        public async Task<TaskResponse?> GetTaskById(Guid? groupId, Guid? taskId)
        {
            if (groupId is null || taskId is null)
            {
                return null;
            }
            var ToDoTask = await _context.GetTaskById(groupId, taskId);
            if (ToDoTask == null)
                return null;

            return ToDoTask?.ToTaskResponse();
        }
        
        public async Task<TaskResponse?> UpdateTask(TaskUpdateRequest? task)
        {
            if (task is null)
            {
                return null;
            }
            var toDoItem = await _context.GetTaskById(task.GroupId, task.TaskId);
            if (toDoItem is null)
            {
                return null;
            }
            var updatedTask = task.ToTask();

            await _context.UpdateTask(updatedTask);
            return updatedTask.ToTaskResponse();
        }

        public async Task<IEnumerable<TaskResponse>?> GetAllCompletedTask()
        {
            var allGroups = await _groupService.GetGroups();
            List<List<TaskResponse>>? tasks = [];
            foreach (var group in allGroups)
            {
                tasks.Add((await GetAllGroupTasks(group.GroupId)).Where(task => task.Success == true).ToList());
            }
            if (tasks.Count == 0)
            {
                return null;
            }
                return tasks.SelectMany(x => x).ToList();
        }

        public async Task<bool> DeleteTask(ToDoItem toDoItem)
        {
            if (toDoItem is null)
            {
                return false;
            }

            var task = await _context.GetTaskById(toDoItem.GroupId, toDoItem.ItemId);
            if (task is null)
            {
                return false;
            }

            await _context.DeleteTask(task);
            return true;
        }

        public async Task<bool> CheckAsCompleted(Guid? taskId)
        {
            if (taskId is null) return false;

            var State = await _context.CheckAsCompleted(taskId);
            return State;
        }
    }
}