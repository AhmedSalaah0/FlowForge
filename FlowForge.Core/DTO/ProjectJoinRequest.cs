using FlowForge.Core.Domain.IdentityEntities;
using FlowForge.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.DTO
{
    public class ProjectJoinRequest
    {
        public Guid ProjectId { get; set; }
        public Guid? AddingUserId { get; set; }
        public ApplicationUser? AddingUser { get; set; }
        public Guid? AddedUserId { get; set; }
        public ApplicationUser? AddedUser { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public ProjectRole MemberRole { get; set; } = ProjectRole.Member;
        public bool IsApproved { get; set; } = false;
    }
}
