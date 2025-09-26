using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Infrastructure.DatabaseContext;
using FlowForge.Core.Enums;

namespace FlowForge.Infrastructure.Repositories;
public class ProjectRepository(ApplicationDbContext context) : IProjectRepository
{
    public async Task<Project> CreateProject(Project project)
    {
        await context.AddAsync(project);
        await context.SaveChangesAsync();
        return project;
    }

    public async Task<Project?> GetProjectById(Guid? projectId)
    {
        Project? project = await context.Projects
            .Include(g => g.ProjectMembers)
            .FirstOrDefaultAsync(g => g.ProjectId == projectId);
        return project;
    }

    public async Task<List<Project>> GetProjects(Guid userId)
    {
        return await context.Projects
            .Include(g => g.ProjectMembers)
            .Where(g => g.ProjectMembers.Any(g => g.MemberId == userId && (g.MembershipStatus == MembershipStatus.ACCEPTED || g.MemberRole == ProjectRole.Creator)))
            .OrderBy(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<Project?> UpdateProject(Guid? projectId, Project project)
    {
        var projectToUpdate = await context.Projects.FirstOrDefaultAsync(g => g.CreatedById == project.CreatedById && g.ProjectId == projectId);
        if(projectToUpdate == null)
            return null;

        project.CreatedAt = projectToUpdate.CreatedAt;
        context.Entry(projectToUpdate).CurrentValues.SetValues(project);
        await context.SaveChangesAsync();
        return project;
    }

    public async Task<bool> DeleteProject(Project project)
    {
        await context.Entry(project)
               .Collection(p => p.ProjectMembers).LoadAsync();

        await context.Entry(project)
            .Collection(p => p.Sections)
            .Query()
            .Include(s => s.Tasks)
            .LoadAsync();

        foreach (var section in project.Sections)
        {
            context.Tasks.RemoveRange(section.Tasks);
        }

        var projectTasks = await context.Tasks
            .Where(t => t.ProjectId == project.ProjectId && t.SectionId == null)
            .ToListAsync();
        context.Tasks.RemoveRange(projectTasks);

        context.ProjectMembers.RemoveRange(project.ProjectMembers);

        context.Sections.RemoveRange(project.Sections);

        context.Projects.Remove(project);

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<ProjectMember>> GetProjectMembers(Guid? ProjectId)
    {
        return await context.ProjectMembers
            .Where(gu => gu.ProjectId == ProjectId)
            .Include(gu => gu.Member)
            .Include(gu => gu.Project)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectMember>> GetAcceptedProjectMembers(Guid? ProjectId)
    {
        return await context.ProjectMembers
            .Where(gu => gu.ProjectId == ProjectId && gu.MembershipStatus == MembershipStatus.ACCEPTED)
            .Include(gu => gu.Member)
            .Include(gu => gu.Project)
            .ToListAsync();
    }

    public async Task<ProjectMember?> GetProjectMemberById(Guid? ProjectId, Guid? MemberId)
    {
        return await context.ProjectMembers
            .Where(gu => gu.ProjectId == ProjectId && gu.MemberId == MemberId)
            .Include(gu => gu.Member)
            .Include(gu => gu.Project)
            .FirstOrDefaultAsync();
    }
    public async Task<bool> AddProjectMember(ProjectMember projectMember)
    {
        ArgumentNullException.ThrowIfNull(projectMember);
        (await context.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectMember.ProjectId))?.ProjectMembers.Add(projectMember);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SendNotification(Notification notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        context.Notifications.Add(notification);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Notification>> GetNotifications(Guid userId)
    {
        return await context.Notifications
            .Where(n => n.ReceiverId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<Project> ChangeVisibility(Project project, ProjectVisibility visibility)
    {
        context.Entry(project).Property(p => p.ProjectVisibility).CurrentValue = visibility;
        await context.SaveChangesAsync();
        return project;
    }
}