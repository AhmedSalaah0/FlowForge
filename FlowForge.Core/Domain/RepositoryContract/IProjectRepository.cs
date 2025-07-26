using FlowForge.Core.Domain.Entities;

namespace FlowForge.Core.Domain.RepositoryContract;
public interface IProjectRepository
{
    Task<Project> CreateProject(Project group);
    Task<List<Project>> GetProjects(Guid userId);
    Task<Project?> GetProjectById(Guid? userId, Guid? id);
    Task<Project?> UpdateProject(Guid? groupId, Project group);
    Task<bool> DeleteProject(Guid userId, Guid groupId);
    Task<IEnumerable<ProjectMember>> GetProjectMembers(Guid? groupId);
}