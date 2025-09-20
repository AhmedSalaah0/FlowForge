using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.DTO
{
    public class ReorderRequest
    {
        public Guid ProjectId { get; set; }
        public Guid SectionId { get; set; }
        public Guid TaskId { get; set; }
        public Guid? PrevTaskId { get; set; }
        public Guid? NextTaskId { get; set; }
    }
}
