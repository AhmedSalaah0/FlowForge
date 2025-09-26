using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Core.DTO;
using FlowForge.Core.ServiceContracts;

public class ReorderTaskService : IReorderTaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly decimal Step = 1024M;
    private readonly decimal MinGap = 0.0000000001M;

    public ReorderTaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<bool> ReorderTask(ReorderRequest reorderRequest, Guid userId)
    {
        if (reorderRequest == null) throw new ArgumentNullException(nameof(reorderRequest));
        if (reorderRequest.ProjectId == Guid.Empty || reorderRequest.SectionId == Guid.Empty)
            throw new ArgumentException("Invalid project or section id");

        using var transaction = await _taskRepository.BeginTransactionAsync();

        var task = await _taskRepository.GetTaskById(reorderRequest.ProjectId, reorderRequest.TaskId)
                   ?? throw new KeyNotFoundException("Task not found");

        var oldSectionId = task.SectionId;
        bool sectionChanged = task.SectionId != reorderRequest.SectionId;

        if (sectionChanged)
        {
            task.SectionId = reorderRequest.SectionId;
        }

        var prev = reorderRequest.PrevTaskId == Guid.Empty ? null :
                   await _taskRepository.GetTaskById(reorderRequest.ProjectId, reorderRequest.PrevTaskId, false);

        var next = reorderRequest.NextTaskId == Guid.Empty ? null :
                   await _taskRepository.GetTaskById(reorderRequest.ProjectId, reorderRequest.NextTaskId, false);

        decimal newOrder;

        if (prev != null && next != null)
        {
            if ((next.Order ?? 0) - (prev.Order ?? 0) < MinGap)
            {
                await ReBalanceSectionTasks(reorderRequest.ProjectId, reorderRequest.SectionId);
                prev = await _taskRepository.GetTaskById(reorderRequest.ProjectId, reorderRequest.PrevTaskId, false);
                next = await _taskRepository.GetTaskById(reorderRequest.ProjectId, reorderRequest.NextTaskId, false);
            }
            newOrder = ((prev?.Order ?? 0) + (next?.Order ?? Step)) / 2m;
        }
        else if (prev != null)
        {
            newOrder = (prev.Order ?? 0) + Step;
        }
        else if (next != null)
        {
            newOrder = (next.Order ?? Step) - Step;
        }
        else
        {
            // empty section
            newOrder = Step;
        }

        task.Order = newOrder;
        await _taskRepository.SaveChangesAsync();

        if (sectionChanged)
        {
            await ReBalanceSectionTasks(reorderRequest.ProjectId, oldSectionId); 
        }

        await transaction.CommitAsync();

        return true;
    }

    public async Task ReBalanceSectionTasks(Guid projectId, Guid? sectionId)
    {
        var sections = await _taskRepository.GetTasks(Guid.Empty, projectId);

        var tasks = sectionId == null ? 
            sections.SelectMany(s => s.Tasks).Where(t => t.SectionId == null).OrderBy(t => t.Order).ToList()
            : sections.FirstOrDefault(s => s.SectionId == sectionId)?.Tasks.OrderBy(t => t.Order).ToList();


        decimal order = Step;
        foreach (var task in tasks)
        {
            task.Order = order;
            order += Step;
            _taskRepository.UpdateTaskOrder(task);
        }

        await _taskRepository.SaveChangesAsync();
    }
}
