using FlowForge.Core.Domain.Entities;
using FlowForge.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.Domain.RepositoryContract
{
    public interface IProjectMemberRepository
    {
        Task AddProjectMember(ProjectMember projectMember);
        Task<bool> RemoveProjectMember(ProjectMember projectMember);
        Task AcceptProjectMember(ProjectMember projectMember);
        Task RejectProjectMember(ProjectMember projectMember);
    }
}
