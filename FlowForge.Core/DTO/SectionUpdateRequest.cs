using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.DTO
{
    public class SectionUpdateRequest
    {
        public Guid SectionId { get; set; }
        public Guid ProjectId { get; set; }
        public string? SectionName { get; set; }
    }
}
