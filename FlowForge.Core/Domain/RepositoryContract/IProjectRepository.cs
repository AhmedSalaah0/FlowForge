using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Enums;

namespace FlowForge.Core.Domain.RepositoryContract;
public interface IProjectRepository
{
    Task<Project> CreateProject(Project group);
    Task<List<Project>> GetProjects(Guid userId);
    Task<Project?> GetProjectById(Guid? projectId);
    Task<Project?> UpdateProject(Guid? groupId, Project group);
    Task<bool> DeleteProject(Project project);
    Task<IEnumerable<ProjectMember>> GetProjectMembers(Guid? groupId);
    Task<ProjectMember?> GetProjectMemberById(Guid? ProjectId, Guid? MemberId);
    Task<bool> AddProjectMember(ProjectMember projectMember);
    Task <Project> ChangeVisibility(Project project, ProjectVisibility visibility);
}