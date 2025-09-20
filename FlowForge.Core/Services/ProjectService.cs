using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Enums;
using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Core.DTO;
using FlowForge.Core.ServiceContracts;
using FlowForge.Core.Domain.IdentityEntities;

namespace FlowForge.Core.Services
{
    public class ProjectService(IProjectRepository context, IProjectMemberRepository projectMemberRepository) : IProjectService
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
            var group = await context.CreateProject(project);
            return project.ToProjectResponse(groupAddRequest.CreatedById);
        }

        public async Task<List<ProjectResponse>> GetProjects(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId is Empty", nameof(userId));
            }

            List<Project>? projects = await context.GetProjects(userId);
            return [.. projects.Select(p => p.ToProjectResponse(userId))];
        }

        public async Task<ProjectResponse> GetProjectById(Guid userId, Guid? projectId)
        {
            if (projectId == Guid.Empty || userId == Guid.Empty)
            {
                throw new ArgumentException("projectId or userId is Empty");
            }
            var projectMembers = await context.GetProjectMembers(projectId);
            var project = await context.GetProjectById(userId, projectId) ?? throw new KeyNotFoundException("projectId or userId is invalid");
            if (!projectMembers.Any(u => u.MemberId == userId) && project.CreatedById != userId)
            {
                throw new UnauthorizedAccessException("You are not a member of this project.");
            }

            return project.ToProjectResponse(userId);
        }
        public async Task<ProjectResponse> UpdateProject(Guid? projectId, ProjectUpdateRequest projectUpdateRequest)
        {
            var project = await context.GetProjectById(projectUpdateRequest.UserId, projectId);
            if (project == null)
            {
                throw new ArgumentException(nameof(project));
            }

            await context.UpdateProject(project.ProjectId, projectUpdateRequest.ToProject());
            return project.ToProjectResponse(projectUpdateRequest.UserId);
        }
        public async Task<bool> DeleteProject(Guid userId, Guid projectId)
        {
            if (projectId == Guid.Empty) return false;


            var group = await context.GetProjectById(userId, projectId);
            if (group == null) return false;
            if (group.CreatedById != userId)
            {
                if (!group.ProjectMembers.Any(u => u.MemberId == userId))
                {
                    throw new UnauthorizedAccessException("You are not authorized to delete this project.");
                }
                var response = await projectMemberRepository.RemoveProjectMember(group.ProjectMembers.FirstOrDefault(u => u.MemberId == userId && u.ProjectId == projectId) ?? throw new ArgumentException("Member not found in the project."));
                return response;
            }
            bool result = await context.DeleteProject(userId, projectId);
            return result;
        }
        public Task<IEnumerable<ProjectMember>> GetProjectMembers(Guid? projectId)
        {
            if (projectId == null)
            {
                throw new ArgumentNullException(nameof(projectId));
            }
            return context.GetProjectMembers(projectId);
        }

        public async Task<ProjectMember> GetProjectMember(Guid? projectId, Guid memberId)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentNullException();
            }
            var projectMember = await context.GetProjectMemberById(projectId, memberId);
            if (projectMember == null)
            {
                throw new ArgumentException(nameof(projectId));
            }
            return projectMember;
        }
    }
}
