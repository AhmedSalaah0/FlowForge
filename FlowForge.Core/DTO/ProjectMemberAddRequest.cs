using FlowForge.Core.Enums;
using FlowForge.Core.Domain.Entities;
namespace FlowForge.Core.DTO
{
    public class ProjectMemberAddRequest
    {
        public Guid ProjectId { get; set; }
        public Guid MemberId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public ProjectRole MemberRole { get; set; } = ProjectRole.Member;
    }
    public static class ProjectMemberAddRequestExtensions
    {
        public static ProjectMember ToProjectMember(this ProjectMemberAddRequest request)
        {
            return new ProjectMember
            {
                ProjectId = request.ProjectId,
                MemberId = request.MemberId,
                MemberRole = request.MemberRole,
            };
        }
    }
}
