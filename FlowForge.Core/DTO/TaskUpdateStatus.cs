using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.DTO
{
    public class TaskUpdateStatus
    {
        public Guid TaskId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
