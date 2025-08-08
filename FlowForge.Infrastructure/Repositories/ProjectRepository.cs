using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Infrastructure.DatabaseContext;
using FlowForge.Core.Enums;

namespace FlowForge.Infrastructure.Repositories;
public class ProjectRepository(ApplicationDbContext context) : IProjectRepository
{
    private readonly ApplicationDbContext _context = context;
    public async Task<Project> CreateProject(Project project)
    {
        await _context.AddAsync(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<Project?> GetProjectById(Guid? userId, Guid? id)
    {
        Project? project = await _context.Projects
            .Include(g => g.ProjectMembers)
            .FirstOrDefaultAsync(g => g.ProjectId == id && (g.CreatedById == userId || g.ProjectMembers.Any(u => u.MemberId == userId)));
        return project;
    }

    public async Task<List<Project>> GetProjects(Guid userId)
    {
        return await _context.Projects
            .Include(g => g.ProjectMembers)
            .Where(g => g.ProjectMembers.Any(g => g.MemberId == userId && (g.MembershipStatus == MembershipStatus.ACCEPTED || g.MemberRole == ProjectRole.Creator)))
            .OrderBy(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<Project?> UpdateProject(Guid? projectId, Project project)
    {
        var projectToUpdate = await _context.Projects.FirstOrDefaultAsync(g => g.CreatedById == project.CreatedById && g.ProjectId == projectId);
        if(projectToUpdate == null)
            return null;

        project.CreatedAt = projectToUpdate.CreatedAt;
        _context.Entry(projectToUpdate).CurrentValues.SetValues(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<bool> DeleteProject(Guid userId, Guid ProjectId)
    {
        var project = await _context.Projects
            .Include(g => g.ProjectMembers)
            .FirstOrDefaultAsync(g => g.ProjectId == ProjectId &&
            (g.CreatedById == userId || g.ProjectMembers.Any(g => g.ProjectId == ProjectId && g.MemberId == userId)));   
        if (project.CreatedById == userId)
        {
            _context.Projects.Remove(project);
            _context.Notifications.RemoveRange(_context.Notifications.Where(n => n.ProjectId == ProjectId));
        }
        else
        {
            var userProject = project.ProjectMembers.FirstOrDefault(u => u.ProjectId == ProjectId);
            if (userProject is not null)
            {
                _context.ProjectMembers.Remove(userProject);
                _context.Notifications.RemoveRange(_context.Notifications.Where(n => n.ProjectId == ProjectId && n.ReceiverId == userId));
            }
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<ProjectMember>> GetProjectMembers(Guid? ProjectId)
    {
        return await _context.ProjectMembers
            .Where(gu => gu.ProjectId == ProjectId)
            .Include(gu => gu.Member)
            .Include(gu => gu.Project)
            .ToListAsync();
    }

    public async Task<ProjectMember?> GetProjectMemberById(Guid? ProjectId, Guid? MemberId)
    {
        return await _context.ProjectMembers
            .Where(gu => gu.ProjectId == ProjectId && gu.MemberId == MemberId)
            .Include(gu => gu.Member)
            .Include(gu => gu.Project)
            .FirstOrDefaultAsync();
    }
    public async Task<bool> AddProjectMember(ProjectMember projectMember)
    {
        ArgumentNullException.ThrowIfNull(projectMember);
        _context.ProjectMembers.Add(projectMember);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SendNotification(Notification notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Notification>> GetNotifications(Guid userId)
    {
        return await _context.Notifications
            .Where(n => n.ReceiverId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }
}