using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.DTO
{
    public class MoveTaskRequest
    {
        public Guid TaskId { get; set; }
        public Guid NewSectionId { get; set; }
        public Guid ProjectId { get; set; }
    }
}
