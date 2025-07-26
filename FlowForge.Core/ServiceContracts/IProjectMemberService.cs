using FlowForge.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.ServiceContracts
{
    public interface IProjectMemberService
    {
        Task<bool> SendJoinRequest(ProjectJoinRequest projectJoinRequest);
        Task<bool> RemoveProjectMember(Guid projectId, Guid userId);
        Task<bool> AcceptProjectMember(Guid projectId, Guid userId);
        Task<bool> RejectProjectMember(Guid projectId, Guid userId);
    }
}
