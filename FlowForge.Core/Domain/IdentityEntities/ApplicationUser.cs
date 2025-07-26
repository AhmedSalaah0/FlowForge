using FlowForge.Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FlowForge.Core.Domain.IdentityEntities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? PersonName { get; set; }
        public ICollection<ProjectMember> UserGroups { get; set; }
    }
}
