using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.DTO
{
    public class AssignTaskRequest
    {
        public Guid projectId {  get; set; }
        public Guid taskId { get; set; }
        public Guid userId { get; set; }
        public Guid memberId { get; set; }
    }
}
