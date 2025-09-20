using FlowForge.Core.Domain.Entities;
using FlowForge.Core.Domain.IdentityEntities;
using FlowForge.Core.DTO;
using FlowForge.Core.Enums;

namespace FlowForge.Core.DTO
{
    public class ProjectResponse
    {
        public Guid ProjectId { get; set; }
        public string? ProjectTitle { get; set; }
        public Guid CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public string SelectedColor { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<ProjectMember>? ProjectMembers { get; set; } = [];
        public ProjectRole? UserRole { get; set; }
    }
}
public static class ProjectExtensions
{
    public static ProjectResponse ToProjectResponse(this Project project, Guid userId)
    {
        var role = project.ProjectMembers?.FirstOrDefault(g => g.MemberId == userId)?.MemberRole;
        return new ProjectResponse
        {
            ProjectId = project.ProjectId,
            ProjectTitle = project.ProjectTitle,
            SelectedColor = project.SelectedColor,
            ProjectMembers = project.ProjectMembers,
            CreatedAt = project.CreatedAt,
            UserRole = role ?? ProjectRole.Member
        };
    }
}
