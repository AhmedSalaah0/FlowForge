using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.ServiceContracts
{
    public interface IReorderTaskService
    {
        Task<bool> ReorderTask(DTO.ReorderRequest reorderRequest, Guid userId);
    }
}
