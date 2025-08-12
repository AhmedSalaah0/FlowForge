using FlowForge.Core.Enums;
using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Core.DTO;
using FlowForge.Core.ServiceContracts;
namespace FlowForge.Core.Services;

public class TaskService(ITaskRepository taskRepository, IProjectService projectService, INotificationService notificationService) : ITaskService
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    private readonly IProjectService _projectService = projectService;
    private readonly INotificationService _notificationService = notificationService;

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

    public async Task<bool> DeleteTask(TaskResponse task)
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

        await _taskRepository.DeleteTask(task.ToTask());
        return true;
    }

    public async Task<bool> UpdateTaskStatus(TaskUpdateStatus taskUpdateStatus)
    {
        if (taskUpdateStatus == null || !Enum.TryParse<ProjectTaskStatus>(taskUpdateStatus.Status, true, out var statusEnum))
        {
            return false;
        }

        var State = await _taskRepository.UpdateTaskStatus(taskUpdateStatus.TaskId, statusEnum);
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

    public async Task MoveTask(Guid userId, MoveTaskRequest request)
    {
        if (request is null || request.TaskId == Guid.Empty || request.ProjectId == Guid.Empty)
        {
            throw new ArgumentException("Invalid request");
        }
        var task = await _taskRepository.GetTaskById(request.ProjectId, request.TaskId);
        if (task is null)
        {
            throw new ArgumentException("Task not found");
        }
        var project = await _projectService.GetProjectById(userId, request.ProjectId);
        if (project is null)
        {
            throw new ArgumentException("Project not found");
        }
        if (project.ProjectMembers.FirstOrDefault(u => u.MemberId == userId)?.MemberRole == ProjectRole.Member)
        {
            throw new UnauthorizedAccessException("You are not authorized to move tasks in this project.");
        }
        bool result = await _taskRepository.MoveTask(task, request.NewSectionId);
        if (!result)
        {
            throw new InvalidOperationException("Failed to move task. Please check the section ID and try again.");
        }
    }

    public async Task<TaskResponse> AssignTask(AssignTaskRequest assignTask)
    {
        ArgumentNullException.ThrowIfNull(assignTask);
        var project = await _projectService.GetProjectById(assignTask.memberId, assignTask.projectId);
        if (project == null)
        {
            throw new ArgumentException("Invalid projectId");
        }
        
        if (project.ProjectMembers.FirstOrDefault(u => u.MemberId == assignTask.userId)?.MemberRole == ProjectRole.Member)
        {
            throw new UnauthorizedAccessException("You are not authorized to assign tasks in this project.");
        }

        var task = _taskRepository.GetTaskById(assignTask.projectId, assignTask.taskId).Result;
        if (task == null)
        {
            throw new ArgumentException("Invalid taskId");
        }
        var member = project.ProjectMembers.FirstOrDefault(u => u.MemberId == assignTask.memberId);
        if (member == null)
        {
            throw new ArgumentException("Member not found in the project");
        }

        

        var UpdatedTask = await _taskRepository.AssignTask(task, assignTask.memberId);
        if (UpdatedTask == null) {
            throw new InvalidOperationException("can't assign this task");
        }
        _notificationService.SendNotification(new Notification()
        {
            NotificationType = NotificationType.INFO,
            Message = $"Task: {task.Title} in project: {task.Project.ProjectTitle} assigned for you",
            ReceiverId = assignTask.memberId,
            ProjectId = assignTask.projectId,
        });
        return UpdatedTask.ToTaskResponse();
    }
}