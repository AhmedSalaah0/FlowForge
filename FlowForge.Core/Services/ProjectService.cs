using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Enums;
using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Core.DTO;
using FlowForge.Core.ServiceContracts;
using FlowForge.Core.Domain.IdentityEntities;

namespace FlowForge.Core.Services
{
    public class ProjectService(IProjectRepository projectRepository, IProjectMemberService projectMemberService) : IProjectService
    {

        public async Task<ProjectResponse> CreateProject(ProjectAddRequest? groupAddRequest)
        {
            ArgumentNullException.ThrowIfNull(groupAddRequest);
            if (groupAddRequest.CreatedById == Guid.Empty)
            {
                throw new ArgumentException(nameof(groupAddRequest.CreatedById));
            }

            groupAddRequest.CreatedAt = DateTime.Now;
            Project project = groupAddRequest.ToProject();
            project.ProjectId = Guid.NewGuid();
            var projectMember = new ProjectMember
            {
                ProjectId = project.ProjectId,
                MemberId = groupAddRequest.CreatedById,
                Project = project,
                MemberRole = ProjectRole.Creator,
                MembershipStatus = MembershipStatus.ACCEPTED
            };
            project.ProjectMembers.Add(projectMember);
            var group = await projectRepository.CreateProject(project);
            return project.ToProjectResponse(groupAddRequest.CreatedById);
        }

        public async Task<List<ProjectResponse>> GetProjects(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId is Empty", nameof(userId));
            }

            List<Project>? projects = await projectRepository.GetProjects(userId);
            return [.. projects.Select(p => p.ToProjectResponse(userId))];
        }

        public async Task<ProjectResponse> GetProjectById(Guid userId, Guid? projectId)
        {
            if (projectId == Guid.Empty || userId == Guid.Empty)
            {
                throw new ArgumentException("projectId or userId is Empty");
            }
            var projectMembers = await projectRepository.GetProjectMembers(projectId);
            var project = await projectRepository.GetProjectById(projectId) ?? throw new KeyNotFoundException("projectId or userId is invalid");
            if (!projectMembers.Any(u => u.MemberId == userId) && project.ProjectVisibility == ProjectVisibility.Private)
            {
                throw new UnauthorizedAccessException("You do not have access to this project");
            }
            return project.ToProjectResponse(userId);
        }
        public async Task<ProjectResponse> UpdateProject(Guid? projectId, ProjectUpdateRequest projectUpdateRequest)
        {
            var project = await projectRepository.GetProjectById(projectId);
            if (project == null)
            {
                throw new ArgumentException(nameof(project));
            }

            await projectRepository.UpdateProject(project.ProjectId, projectUpdateRequest.ToProject());
            return project.ToProjectResponse(projectUpdateRequest.UserId);
        }
        public async Task<bool> DeleteProject(Guid userId, Guid projectId)
        {
            if (projectId == Guid.Empty) return false;


            var project = await projectRepository.GetProjectById(projectId);
            if (project == null) return false;

            if (project.CreatedById == userId)
            {
                
                bool result = await projectRepository.DeleteProject(project);
                return result;
            }
            return await projectMemberService.RemoveProjectMember(projectId, userId, userId);
        }
        public Task<IEnumerable<ProjectMember>> GetProjectMembers(Guid? projectId)
        {
            if (projectId == null)
            {
                throw new ArgumentNullException(nameof(projectId));
            }
            return projectRepository.GetProjectMembers(projectId);
        }

        public async Task<ProjectMember> GetProjectMember(Guid? projectId, Guid memberId)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentNullException();
            }
            var projectMember = await projectRepository.GetProjectMemberById(projectId, memberId);
            return projectMember == null ? throw new ArgumentException(nameof(projectId)) : projectMember;
        }

        public async Task<bool> AddProjectMember(ProjectMemberAddRequest projectMemberAddRequest)
        {
            ProjectMember projectMember = projectMemberAddRequest.ToProjectMember();
            projectMember.MembershipStatus = MembershipStatus.ACCEPTED;
            return await projectRepository.AddProjectMember(projectMember);
        }

        public async Task<ProjectResponse> ChangeVisibility(ChangeVisibilityRequest changeVisibilityRequest)
        {
            if (!Enum.TryParse<ProjectVisibility>(changeVisibilityRequest.ProjectVisibility, true, out var newVisibility))
            {
                throw new ArgumentException("Invalid visibility value", nameof(changeVisibilityRequest.ProjectVisibility));
            }
            var project = await projectRepository.GetProjectById(changeVisibilityRequest.ProjectId) ?? throw new ArgumentException("Project not found", nameof(changeVisibilityRequest.ProjectId));

            var member = project.ProjectMembers.FirstOrDefault(pm => pm.MemberId == changeVisibilityRequest.UserId);

            if (project.CreatedById != changeVisibilityRequest.UserId && (member == null || member.MemberRole == ProjectRole.Member))
            {
                throw new UnauthorizedAccessException("Only creator or moderator can change visibility");
            }
            return await projectRepository.ChangeVisibility(project, newVisibility)
                .ContinueWith(t => t.Result.ToProjectResponse(Guid.Empty));
        }
    }
}
