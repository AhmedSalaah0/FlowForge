using FlowForge.Core.Domain.Entities;
using FlowForge.Core.DTO;

namespace FlowForge.Core.ServiceContracts
{
    public interface IProjectService
    {
        Task<ProjectResponse> CreateProject(ProjectAddRequest projectAddRequest, Guid userId);
        Task<List<ProjectResponse>> GetProjects(Guid userId);
        Task<ProjectResponse> GetProjectById(Guid userId, Guid? projectId);
        Task<ProjectResponse> UpdateProject(Guid? projectId, ProjectUpdateRequest projectUpdateRequest);
        Task<IEnumerable<ProjectMember>> GetProjectMembers(Guid? projectId);
        Task<bool> DeleteProject(Guid userId, Guid projectId);
    }
}
