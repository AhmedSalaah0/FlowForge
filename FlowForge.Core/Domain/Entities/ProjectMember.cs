using FlowForge.Core.Domain.IdentityEntities;
using FlowForge.Core.Enums;

namespace FlowForge.Core.Domain.Entities
{
    public class ProjectMember
    {
        public Guid MemberId { get; set; }
        public ApplicationUser? Member { get; set; } = null!;
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public ProjectRole MemberRole { get; set; } = ProjectRole.Member;
        public MembershipStatus MembershipStatus { get; set; } = MembershipStatus.PENDING;
    }
}