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

    public async Task<Project?> GetProjectById(Guid? userId, Guid? id)
    {
        Project? project = await context.Projects
            .Include(g => g.ProjectMembers)
            .FirstOrDefaultAsync(g => g.ProjectId == id && (g.CreatedById == userId || g.ProjectMembers.Any(u => u.MemberId == userId)));
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

    public async Task<bool> DeleteProject(Guid userId, Guid ProjectId)
    {
        var project = await context.Projects
            .Include(g => g.ProjectMembers)
            .FirstOrDefaultAsync(g => g.ProjectId == ProjectId &&
            (g.CreatedById == userId)) ?? throw new ArgumentException("Project Not Found");

        context.Projects.Remove(project);
        context.Notifications.RemoveRange(context.Notifications.Where(n => n.ProjectId == ProjectId));
       
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
        context.ProjectMembers.Add(projectMember);
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
}