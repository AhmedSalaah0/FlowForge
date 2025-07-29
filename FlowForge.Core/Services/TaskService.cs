using FlowForge.Core.Enums;
using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Core.DTO;
using FlowForge.Core.ServiceContracts;
namespace FlowForge.Core.Services;

public class TaskService(ITaskRepository taskRepository, IProjectService projectService) : ITaskService
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    private readonly IProjectService _projectService = projectService;

    public async Task<TaskResponse> AddTask(Guid userId, TaskAddRequest? task)
    {
        ArgumentNullException.ThrowIfNull(task);

        var ProjectTask = task.ToTask();
        ProjectTask.CreatedById = userId;
        ProjectTask.TaskId = Guid.NewGuid();
        var project = await _projectService.GetProjectById(userId, task.ProjectId);
        if (project == null)
        {
            throw new ArgumentException("invalid projectId");
        }
        if (project.ProjectMembers.FirstOrDefault(u => u.MemberId == userId)?.MemberRole == ProjectRole.Member)
        {
            throw new UnauthorizedAccessException("You are not authorized to add tasks to this project.");
        }
        await _taskRepository.CreateTask(ProjectTask);
        var taskResponse = ProjectTask.ToTaskResponse();
        return taskResponse;
    }

    public async Task<IEnumerable<SectionWithTasksResponse>?> GetAllProjectTasks(Guid userId, Guid projectId)
    {
        if (projectId == Guid.Empty)
        {
            return null;
        }
        var projectUsers = await _projectService.GetProjectMembers(projectId);
        if (!projectUsers.Any(u => u.MemberId == userId))
        {
            throw new UnauthorizedAccessException("You are not a member of this Project.");
        }
        var Tasks = await _taskRepository.GetTasks(userId, projectId);
        if (Tasks == null)
        {
            return null;
        }
        return [.. Tasks.Select(s => s.ToSectionWithTasksResponse())];
    }

    public async Task<TaskResponse?> GetTaskById(Guid? projectId, Guid? taskId)
    {
        if (projectId is null || taskId is null)
        {
            return null;
        }
        var task = await _taskRepository.GetTaskById(projectId, taskId);
        if (task == null)
            return null;

        return task?.ToTaskResponse();
    }
    
    public async Task<TaskResponse?> UpdateTask(TaskUpdateRequest? task)
    {
        if (task is null)
        {
            return null;
        }
        var toDoItem = await _taskRepository.GetTaskById(task.ProjectId, task.TaskId);
        if (toDoItem is null)
        {
            return null;
        }
        var updatedTask = task.ToTask();

        await _taskRepository.UpdateTask(updatedTask);
        return updatedTask.ToTaskResponse();
    }

    public async Task<IEnumerable<TaskResponse>?> GetAllCompletedTask(Guid userId)
    {
        var allProjectss = await _projectService.GetProjects(userId);
        List<List<TaskResponse>>? tasks = [];
        foreach (var project in allProjectss)
        {
            //tasks.Add([.. (await GetAllProjectTasks(userId, project.ProjectId))?.Where(task => task.Success)]);
        }
        if (tasks.Count == 0)
        {
            return null;
        }
            return tasks.SelectMany(x => x).ToList();
    }

    public async Task<bool> DeleteTask(ProjectTask task)
    {
        if (task is null)
        {
            return false;
        }

        var projectTask = await _taskRepository.GetTaskById(task.ProjectId, task.TaskId);
        if (projectTask is null)
        {
            return false;
        }

        var project = await _projectService.GetProjectById(task.CreatedById, projectTask.ProjectId);
        if (project is null)
        {
            return false;
        }

        await _taskRepository.DeleteTask(task);
        return true;
    }

    public async Task<bool> CheckAsCompleted(Guid? taskId)
    {
        if (taskId is null) return false;

        var State = await _taskRepository.CheckAsCompleted(taskId);
        return State;
    }
    
    public async Task<bool> ScheduleTask(ScheduleTaskRequest request)
    {
        if (request is null || request.TaskId is null || request.ProjectId is null)
        {
            return false;
        }
        var task = await _taskRepository.GetTaskById(request.ProjectId, request.TaskId);
        if (task is null)
        {
            return false;
        }

        DateTime scheduledDateTime = request.ScheduleDate.ToDateTime(request.ScheduleTime);
        
        task.ScheduleDateTime = scheduledDateTime;
        task.IsRecurring = request.IsRecurring;
        task.RecurringInterval = request.IsRecurring ? request.RecurringInterval : 0;
        
        await _taskRepository.UpdateTask(task);
        
        return true;
    }

    public async Task<bool> UnScheduleTask(Guid taskId, Guid projectId, Guid userId)
    {
        if (taskId == Guid.Empty || projectId == Guid.Empty || userId == Guid.Empty)
        {
            return false;
        }
        
        var task = _taskRepository.GetTaskById(projectId, taskId).Result;
        var GroupUsers = _projectService.GetProjectMembers(projectId).Result;
        if (task == null || !GroupUsers.Any(u => u.MemberId == userId))
        {
            return false;
        }
        if (task.CreatedById != userId && GroupUsers.FirstOrDefault(u => u.MemberId == userId && u.ProjectId == projectId)?.MemberRole == ProjectRole.Member)
        {
            return false;
        }
        task.ScheduleDateTime = null;
        task.IsRecurring = false;
        task.RecurringInterval = 0;
        await _taskRepository.UpdateTask(task);
        return true;
    }
}