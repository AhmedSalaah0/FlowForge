using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Enums;
using FlowForge.Core.Domain.RepositoryContract;
using FlowForge.Core.DTO;
using FlowForge.Core.ServiceContracts;
using FlowForge.Core.Domain.IdentityEntities;

namespace FlowForge.Core.Services
{
    public class ProjectService(IProjectRepository context) : IProjectService
    {
        private readonly IProjectRepository _context = context;

        public async Task<ProjectResponse> CreateProject(ProjectAddRequest groupAddRequest, Guid userId)
        {
            ArgumentNullException.ThrowIfNull(groupAddRequest);
            groupAddRequest.CreatedAt = DateTime.Now;
            Project groupTasks = groupAddRequest.ToProject();
            groupTasks.CreatedById = userId;
            var UserGroup = new ProjectMember
            {
                ProjectId = groupTasks.ProjectId,
                MemberId = userId,
                Project = groupTasks,
                MemberRole = ProjectRole.Creator
            };
            groupTasks.ProjectMembers.Add(UserGroup);
            var group = await _context.CreateProject(groupTasks);
            return group.ToProjectResponse(userId);
        }

        public async Task<List<ProjectResponse>> GetProjects(Guid userId)
        {
            List<Project>? projects = await _context.GetProjects(userId);
            return [.. projects.Select(p => p.ToProjectResponse(userId))];
        }

        public async Task<ProjectResponse> GetProjectById(Guid userId, Guid? projectId)
        {
            if (projectId == null)
            {
                throw new ArgumentNullException(nameof(projectId));
            }
            var projectMembers = await _context.GetProjectMembers(projectId);
            var project = await _context.GetProjectById(userId, projectId);
            if (project == null)
            {
                throw new ArgumentException(nameof(project));
            }
            if (!projectMembers.Any(u => u.MemberId == userId) && project.CreatedById != userId)
            {
                throw new UnauthorizedAccessException("You are not a member of this project.");
            }

            return project.ToProjectResponse(userId);
        }
        public async Task<ProjectResponse> UpdateProject(Guid? projectId, ProjectUpdateRequest projectUpdateRequest)
        {
            var project = await _context.GetProjectById(projectUpdateRequest.UserId, projectId);
            if (project == null)
            {
                throw new ArgumentException(nameof(project));
            }

            await _context.UpdateProject(project.ProjectId, projectUpdateRequest.ToProject());
            return project.ToProjectResponse(projectUpdateRequest.UserId);
        }
        public async Task<bool> DeleteProject(Guid userId, Guid projectId)
        {
            if (projectId == Guid.Empty) return false;


            var group = await _context.GetProjectById(userId, projectId);
            if (group == null) return false;
            bool result = await _context.DeleteProject(userId, projectId);
            return result;
        }
        public Task<IEnumerable<ProjectMember>> GetProjectMembers(Guid? projectId)
        {
            if (projectId == null)
            {
                throw new ArgumentNullException(nameof(projectId));
            }
            return _context.GetProjectMembers(projectId);
        }

        public async Task<ProjectMember> GetProjectMember(Guid? projectId, Guid memberId)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentNullException();
            }
            var projectMember = await _context.GetProjectMemberById(projectId, memberId);
            if (projectMember == null)
            {
                throw new ArgumentException(nameof(projectId));
            }
            return projectMember;
        }
    }
}
